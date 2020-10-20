using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SP.Consumers.Models;
using SP.FileStorage.Client.Models;
using SP.FileStorage.Client.Services;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Exceptions;
using SP.Messenger.Application.Messages.Command.SaveSystemMessage;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Domains.Views;
using SP.Messenger.Persistence;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Application.Messages.Command.AttachedFileProtocolFromService
{
    public class AttachedFileProtocolFromServiceCommandHandler : IRequestHandler<AttachedFileProtocolFromServiceCommand, Unit>
    {
        private readonly IFileStorageClientService _fileStorage;
        private readonly MessengerDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public AttachedFileProtocolFromServiceCommandHandler(IFileStorageClientService fileStorage, 
            MessengerDbContext context, 
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _fileStorage = fileStorage;
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
            
            
        }

        public async Task<Unit> Handle(AttachedFileProtocolFromServiceCommand request, CancellationToken token)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            var fileInfo = await _fileStorage.GetFileInfoAsync(request.Link, token);
            if (fileInfo is null)
            {
                Log.Error($"{nameof(AttachedFileProtocolFromServiceCommandHandler)} файл не найден");
                return Unit.Value;
            }
            
            var chatInfo = await GetChatByDocumentId(request.ProtocolId, token);
            if (chatInfo is null)
            {
                Log.Error($"{nameof(AttachedFileProtocolFromServiceCommandHandler)} чат не найден");
                return Unit.Value;
            }

            var messageClient = BuildMessageClient(chatInfo.ChatId, fileInfo, chatInfo, request.Link, currentUser);
            var message = messenger.Create.Message
            (
                chatId:chatInfo.ChatId,
                accountId: 999999,
                messageTypeId:1,
                content:messageClient
            );

            var command = SaveMessageCommand.Create(message, currentUser.Login);
            await _mediator.Send(command, token);

            ////////////////////////////////////////////////////
            var querySql = $@"select * from 'Messages' c
                            where c.'ChatID' = {chatInfo.ChatId}
                            AND c.'Content'->@VotingContent@->> @VotingId@ is not null
                            AND c.'Content'->@VotingContent@->> @VotingId@ <> @00000000-0000-0000-0000-000000000000@"
                 .Replace("'", "\"")
                 .Replace("@", "'");
            
            var formattableString = FormattableStringFactory.Create(querySql, chatInfo.ChatId);
            Log.Information($"formattableString: {formattableString}");
            var messages = _context.Messages.FromSqlInterpolated(formattableString);

            var msg = await messages.FirstOrDefaultAsync(token);
            if (msg is null)
            {
                Log.Error("!!! Сообщение с голосованием не найдено");
                throw new NotFoundException("!!! Сообщение с голосованием не найдено", chatInfo.ChatId);
            }
            var content = msg.Content.FromJson<ContentMessage>();
            if (content.VotingContent is null || content.VotingContent.VotingId == Guid.Empty)
            {
                Log.Error("!!! Неверное сообщение, не содержит голосование");
                throw new NotFoundException("!!! Неверное сообщение, не содержит голосование", msg.MessageId);
            }

            var stateVoting = await _context.StateVotingView
                .Include(x=>x.Account)
                .FirstOrDefaultAsync(x=>x.VotingId == content.VotingContent.VotingId && x.IsLike == false, token);
            if (stateVoting is null)
            {
                Log.Error("!!! Голосование не найдено");
                throw new NotFoundException("!!! Голосование не найдено", content.VotingContent.VotingId);
            }

            var commandSystemMessage = SaveSystemMessageCommand.Create
            (
                accountId: 999999,
                documentId: Guid.Parse(chatInfo.DocumentId),
                message: $"Голосование завершено. Пользователь {stateVoting.Account.LastName} {stateVoting.Account.FirstName} проголосовал Против по причине: {stateVoting.Comment}",
                mnemonic: chatInfo.Mnemonic
            );
            await _mediator.Send(commandSystemMessage, token);
            ////////////////////////////////////////////////////
            
            return Unit.Value;
        }
        
        private async Task<ChatView> GetChatByDocumentId(Guid documentId, CancellationToken cancellationToken)
            => await _context.ChatView
                .FirstOrDefaultAsync(x => x.DocumentId.Equals(documentId.ToString()), cancellationToken);
        
        private static MessageClient BuildMessageClient(long chatId, FileDto file, ChatView chat, string link, ICurrentUser currentUser)
        {
            var module = (ModuleName)Enum.Parse(typeof(ModuleName), chat.Module);
            return new MessageClient
            {
                Commands = Array.Empty<CommandClient>(),
                Content = string.Empty,
                Date = DateTime.UtcNow,
                DocumentId = Guid.Parse(chat.DocumentId),
                File = new MessageFile
                {
                    Extension = file.Extension,
                    Filename = file.Name,
                    Url = link,
                    Lenght = Convert.ToInt32(file.Length),
                    ContentType = file.ContentType
                },
                ButtonCommands = Array.Empty<ButtonCommand>(),
                ModuleName = module,
                ChatTypeMnemonic = chat.Mnemonic,
                ChatId = chatId,
                MessageId = 1,
                Author = new Author 
                (
                    currentUser.Id,
                    currentUser.Login,
                    currentUser.FirstName,
                    currentUser.LastName,
                    currentUser.MiddleName
                ),
                MessageType = MessageTypeClient.User,
                Edited = false,
                Pined = false,
                Readed = false,
                VotingClient = null
            };
        }
    }
}