using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Chats.Queries.GetChatById;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Application.MessengerAssistent.Notifications;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Common.Implementations;
using SP.Messenger.Common.Settings;
using SP.Messenger.Domains.Entities;
using CommandClient = SP.Consumers.Models.CommandClient;
using MessageFile = SP.Consumers.Models.MessageFile;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Application.MessengerAssistent.Commands
{
    public class MessengerAssistentCommandHandler : IRequestHandler<MessengerAssistentCommand, ProcessingResult<bool>>
    {
        private readonly IMediator _mediator;
        private readonly IOptions<Settings> _options;

        public MessengerAssistentCommandHandler(IMediator mediator, IOptions<Settings> options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<ProcessingResult<bool>> Handle(MessengerAssistentCommand request,
            CancellationToken cancellationToken)
        {
            var command = Of(request.Message, request.Message.UserName);
            Log.Information($"MessengerAssistentCommandHandler command: {command.ToJson()}");
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.Result != 0)
            {
                var requestQuery = GetChatByIdQuery.Create(request.Message.Header.InformationChat.ChatId);
                var chat = await _mediator.Send(requestQuery,cancellationToken);
                
                request.Message.MessageClient.MessageId = result.Result;
                request.Message.MessageClient.Author = Author.Create(request.Message.Header.Account.Id,
                    request.Message.Header.Account.Email,request.Message.Header.Account.FirstName,
                    request.Message.Header.Account.LastName,request.Message.Header.Account.MiddleName);
                
                request.Message.Header.InformationChat.ParentDocumentId = chat?.ParentDocumentId;
                
                if(request.Message.MessageClient.File != null)
                    request.Message.MessageClient.File.Url 
                     = $"{_options.Value.FileServer.Storage}{request.Message.MessageClient?.File?.Url?.Replace(@"\","/")}";
                
                Log.Information($"NotificationUsers: {request.Message.ToJson()}");
                await NotificationUsers(request.Message, cancellationToken);
            }

            return new ProcessingResult<bool>(result.Result > 0);
        }

        private IRequest<ProcessingResult<long>> Of(MessengerServerEvent @event, string userName)
        {
            Message message;
            switch (@event.Header.MessageType)
            {
                case Market.EventBus.RMQ.Shared.Events.MessageType.Bot:
                    @event.MessageClient.Content = "Команда бота";
                    message = messenger.Create.Message(@event.Header?.InformationChat?.ChatId ?? 0,
                        @event.Header?.Account?.Id ?? 0,
                        (int) Enum.Parse(@event.Header?.MessageType.GetType(),
                            @event.Header?.MessageType.ToString()),
                        @event.MessageClient);
                    return SaveMessageCommand.Create(message, userName);

                case Market.EventBus.RMQ.Shared.Events.MessageType.User:
                    message = messenger.Create.Message(@event.Header?.InformationChat?.ChatId ?? 0,
                        @event.Header?.Account?.Id ?? 0,
                        (int) Enum.Parse(@event.Header?.MessageType.GetType(),
                            @event.Header?.MessageType.ToString()),
                        @event.MessageClient);
                    return SaveMessageCommand.Create(message, userName);
                case Market.EventBus.RMQ.Shared.Events.MessageType.System:
                    message = messenger.Create.Message(@event.Header?.InformationChat?.ChatId ?? 0,
                        @event.Header?.Account?.Id ?? 0,
                        (int) Enum.Parse(@event.Header?.MessageType.GetType(),
                            @event.Header?.MessageType.ToString()),
                        @event.MessageClient);
                    return SaveMessageCommand.Create(message, userName);
                case Market.EventBus.RMQ.Shared.Events.MessageType.File:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            message = messenger.Create.Message(@event.Header?.InformationChat?.ChatId ?? 0,
                @event.Header?.Account?.Id ?? 0,
                (int) Enum.Parse(@event.Header?.MessageType.GetType(),
                @event.Header?.MessageType.ToString()),
                @event.MessageClient);
            return default;
        }

        private Task NotificationUsers(MessengerServerEvent @event, CancellationToken cancellationToken)
        {
            var _event = BuildEventMessage(@event);
            Log.Information($"******{nameof(NotificationUsers)} event: {@event.ToJson()}");
            return _mediator.Publish(UserNotification.Create(_event), cancellationToken);
        }
        
        private MessengerClientEvent BuildEventMessage(MessengerServerEvent @event)
        {
            var commands = CommandClient.CreateArray(@event.Header.Command?.Command, @event.Header.Command?.Value,
                @event.Header.Command?.DisplayName, @event.Header.Command?.BotMessageType, AvailabilityCommand.Open,
                @event.Header.Command?.Manuals);

            var message = new MessageClient
            {
                Author = @event.MessageClient?.Author,
                Date = @event.MessageClient?.Date ?? DateTime.UtcNow,
                ChatId = @event.MessageClient?.ChatId ?? 0,
                Content = @event.MessageClient?.Content,
                DocumentId = @event.MessageClient?.DocumentId ?? Guid.Empty,
                ChatTypeMnemonic = @event.MessageClient?.ChatTypeMnemonic,
                MessageType = @event.MessageClient?.MessageType ?? MessageTypeClient.User,
                Commands = null,
                ButtonCommands = null,
                File = @event.MessageClient?.File ?? new MessageFile( ),
                MessageId = @event.MessageClient?.MessageId ?? 0
            };
            
            var _event = MessengerClientEvent.Create(@event.Header,
                @event.Header.Account.ConnectionId,
                string.Empty, message);

            return _event;
        }
    }
}
