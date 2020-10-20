using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.FileStorage.Client.Services;
using SP.Market.Core.Extension;
using SP.Messenger.Application.Accounts.Commands.SaveAccountRange;
using SP.Messenger.Application.Chats.Builders;
using SP.Messenger.Application.Chats.Queries.GetChatType;
using SP.Messenger.Common.Contracts;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Commands.CreateOrganizationChat
{
    public class CreateOrganizationChatCommandHandler : IRequestHandler<CreateOrganizationChatCommand, ProcessingResult<long>>
    {
        private readonly MessengerDbContext _context;
        private readonly IMediator _mediator;
        private readonly IFileStorageClientService _fileStorage;

        public CreateOrganizationChatCommandHandler(MessengerDbContext context, IMediator mediator, IFileStorageClientService fileStorage)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        public async Task<ProcessingResult<long>> Handle(CreateOrganizationChatCommand request, CancellationToken cancellationToken)
        {
            Log.Information(
                $"{nameof(CreateOrganizationChatCommandHandler)} ChatTypeMnemonic: {request.ChatTypeMnemonic}");
            var chatTypeDto = await _mediator.Send(GetChatTypeMnemonicQuery.Create(request.ChatTypeMnemonic),
                cancellationToken);
            Log.Information($"{nameof(CreateOrganizationChatCommandHandler)} chatTypeDto: {chatTypeDto?.ToJson()}");

            var chat = Create.Chat(request.Name, chatTypeDto.ChatTypeId, request.ChatData, true);

            var chatDb = await _context.ChatView
                .Where(x => x.Mnemonic.Equals(chatTypeDto.Mnemonic) && x.DocumentId.Equals(request.ChatData.DocumentId))
                .ToArrayAsync(cancellationToken);

            if (chatDb != null && chatDb.Any())
            {
                return new ProcessingResult<long>(chatDb.FirstOrDefault()?.ChatId ?? 0);
            }

            var modelDbChat = await _context.Chats.AddAsync(chat, cancellationToken);

            var resultSaveChat = await _context.SaveChangesAsync(cancellationToken);
            var chatId = modelDbChat.Entity.ChatId;

            Log.Information($"save {nameof(Accounts)} {request.Accounts.ToJson()}");

            var models = request.Accounts?.Select(x => new Account
            {
                AccountId = x.AccountId,
                FirstName = x.FirstName,
                IsDisabled = false,
                LastName = x.LastName,
                MiddleName = x.MiddleName,
                Login = x.Login
            }).ToArray();

            var command = SaveAccountRangeCommand.Create(models);
            await _mediator.Send(command, cancellationToken);

            var dateNow = DateTime.UtcNow;

            var chatNewDb = await _context.Chats.FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);

            await AddAccountsToChat(request, cancellationToken, chatNewDb, chatId, dateNow);

            if (resultSaveChat > 0)
            {
                await _fileStorage.CreateBucketAsync(chatNewDb.Name, cancellationToken);
                return new ProcessingResult<long>(chatId);
            }

            return new ProcessingResult<long>(0, new IError[] {new SimpleResponseError("не удалось"),});
        }

        private async Task AddAccountsToChat(CreateOrganizationChatCommand request, CancellationToken cancellationToken,
            Chat chatNewDb, long chatId, DateTime dateNow)
        {
            try
            {
                foreach (var account in request.Accounts)
                {
                    if (account is null)
                    {
                        continue;
                    }

                    chatNewDb.Accounts.Add(new AccountChat
                    {
                        ChatId = chatId,
                        AccountId = account.AccountId,
                        UnionUserDate = dateNow
                    });
                }

                Log.Information($"save {nameof(AccountChat)} {chatNewDb.Accounts.ToJson()}");
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error($"save {nameof(AccountChat)} {ex}");
            }
        }
    }
}
