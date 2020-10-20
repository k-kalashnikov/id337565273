using MediatR;
using Microsoft.EntityFrameworkCore;
using SP.Consumers.Models;
using SP.FileStorage.Client.Models;
using SP.FileStorage.Client.Services;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Domains.Views;
using SP.Messenger.Persistence;
using System;
using System.Threading;
using System.Threading.Tasks;
using CommandClient = SP.Consumers.Models.CommandClient;
using MessageFile = SP.Consumers.Models.MessageFile;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Application.Messages.Command.AttachedFile
{
    public class AttachedFileCommandHandler : IRequestHandler<AttachedFileCommand, ProcessingResult<long>>
    {
        private readonly IFileStorageClientService _fileStorage;
        private readonly MessengerDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public AttachedFileCommandHandler(IFileStorageClientService fileStorage,
            MessengerDbContext context,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _fileStorage = fileStorage;
            _context = context;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<ProcessingResult<long>> Handle(AttachedFileCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            var fileInfo = await _fileStorage.GetFileInfoAsync(request.Link, cancellationToken);
            if (fileInfo is null)
                return new ProcessingResult<long>(default, new[] { new SimpleResponseError("Файл не найден") });

            var chatInfo = await GetChatById(request.ChatId, cancellationToken);
            if (chatInfo is null)
                return new ProcessingResult<long>(default, new[] { new SimpleResponseError("Чат не найден") });

            var messageClient = BuildMessageClient(request.ChatId, fileInfo, chatInfo, request.Link, currentUser);
            var message = messenger.Create.Message(request.ChatId,
                        currentUser.Id,
                        1,
                        messageClient);

            var command = SaveMessageCommand.Create(message, currentUser.Login);
            var result = await _mediator.Send(command, cancellationToken);

            return result.Result != default
                ? new ProcessingResult<long>(result.Result)
                : new ProcessingResult<long>(default, result.Errors);
        }

        private async Task<ChatView> GetChatById(long chatId, CancellationToken cancellationToken)
        {
            return await _context.ChatView
                .FirstOrDefaultAsync(x => x.ChatId.Equals(chatId), cancellationToken);
        }

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
