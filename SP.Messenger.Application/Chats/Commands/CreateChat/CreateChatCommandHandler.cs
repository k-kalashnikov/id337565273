using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.FileStorage.Client.Services;
using SP.Market.Core.Extension;
using SP.Messenger.Application.Accounts.Commands.SaveAccountRange;
using SP.Messenger.Application.Chats.Builders;
using SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccounts;
using SP.Messenger.Application.Chats.Queries.GetChats;
using SP.Messenger.Application.Chats.Queries.GetChatType;
using SP.Messenger.Common.Contracts;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Entities;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Chats.Commands.CreateChat
{
    public class CreateChatCommandHandler : IRequestHandler<CreateChatCommand, ProcessingResult<long>>
    {
        private readonly MessengerDbContext _context;
        private readonly IMediator _mediator;
        private readonly IFileStorageClientService _fileStorage;

        public CreateChatCommandHandler(MessengerDbContext context, IMediator mediator, IFileStorageClientService fileStorage)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        public async Task<ProcessingResult<long>> Handle(CreateChatCommand request, CancellationToken cancellationToken)
        {
         
                Log.Information($"{nameof(CreateChatCommandHandler)} ChatTypeMnemonic: {request.ChatTypeMnemonic}");
                var chatTypeDto = await _mediator.Send(GetChatTypeMnemonicQuery.Create(request.ChatTypeMnemonic), cancellationToken);
                Log.Information($"{nameof(CreateChatCommandHandler)} chatTypeDto: {chatTypeDto?.ToJson()}");

                long? parentChatId = null;
                if (!string.IsNullOrWhiteSpace(request.Data?.ParentDocumentId))
                {
                    var parentChatDb = await _context.ChatView
                        .FirstOrDefaultAsync(
                        x => x.DocumentId == request.Data.ParentDocumentId,
                        cancellationToken);
                    parentChatId = parentChatDb?.ChatId;
                }

                var chat = Create.Chat(request.Name, chatTypeDto.ChatTypeId, request.Data, request.IsActive, parentChatId);

                //Проверка на существование чата
                var chatDb = await _context.ChatView.Where(x => x.Mnemonic.Equals(chatTypeDto.Mnemonic)
                                                                && x.DocumentId.Equals(request.Data.DocumentId))
                    .ToArrayAsync(cancellationToken);

                if (chatDb != null && chatDb.Any())
                    return new ProcessingResult<long>(chatDb.FirstOrDefault()?.ChatId ?? 0);


                var modelDbChat = await _context.Chats.AddAsync(chat, cancellationToken);

                var resultSaveChat = await _context.SaveChangesAsync(cancellationToken);
                var chatId = modelDbChat.Entity.ChatId;

                var document = Create.Document((Guid.Parse(request.Data?.DocumentId ?? Guid.Empty.ToString())), request.Data?.DocumentTypeId ?? 0);

                var doc = await _context.Documents.FirstOrDefaultAsync(
                    x => x.DocumentId == document.DocumentId, cancellationToken);

                if (doc is null)
                {
                    await _context.Documents.AddAsync(document, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var chatNewDb = await _context.Chats.FirstOrDefaultAsync(x => x.ChatId == chatId, cancellationToken);

                chatNewDb.Documents.Add(new ChatDocument
                {
                    ChatId = chatId,
                    DocumentId = document.DocumentId
                });

                await _context.SaveChangesAsync(cancellationToken);

                if ((request.Accounts?.Count ?? 0) != 0) 
                {
                    Log.Information($"save {nameof(Accounts)} {request.Accounts.ToJson()}");
                    await SaveAccounts(request.Accounts, cancellationToken);
                }


                var dateNow = DateTime.UtcNow;

                try
                {
                    foreach (var account in request.Accounts)
                    {
                        if (account is null)
                            continue;
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
                    Log.Error($"save {nameof(AccountChat)} {ex.ToString()}");
                }

                if (resultSaveChat > 0)
                {
                    await UpdateAccounts(request.Data?.Module,
                        request.Data?.DocumentId, chatId);

                    await _fileStorage.CreateBucketAsync($"bucket-chat-{chatId}", cancellationToken);

                    return new ProcessingResult<long>(chatId);
                }

                return new ProcessingResult<long>(0, new IError[] { new SimpleResponseError("не удалось"), });
        }

        private async Task SaveAccounts(IEnumerable<AccountMessengerDTO> accounts, CancellationToken cancellationToken)
        {
            var models = accounts?.Select(x => new Account
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
        }

        private async Task UpdateAccounts(string module, string documentId, long chatId)
        {
            if (!module.Equals("BlueReport"))
                return;

            var command = UpdateAccountsCommand.Create(Guid.Parse(documentId), chatId);
            await _mediator.Send(command);
        }
    }
}
