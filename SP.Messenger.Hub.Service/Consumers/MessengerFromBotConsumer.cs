using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Builder;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Hub.Service.Hub;
using SP.Messenger.Hub.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SP.Messenger.Application.Accounts.Queries.GetAccountsChatById;
using SP.Messenger.Application.Chats.Queries.GetChatById;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Hub.Service.Consumers
{
    public class MessengerFromBotConsumer : IConsumer<MessengerBotClientEvent>
    {
        private readonly IHubContext<MessengerHub> _messenger;
        private readonly IActiveUsersService _activeUsersService;
        private readonly IMediator _mediator;
        private readonly IBusControl _bus;

        public MessengerFromBotConsumer(IHubContext<MessengerHub> messenger,
            IActiveUsersService activeUsersService, IMediator mediator, IBusControl bus)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _activeUsersService = activeUsersService ?? throw new ArgumentNullException(nameof(activeUsersService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task Consume(ConsumeContext<MessengerBotClientEvent> context)
        {
            var @event = MessengerEvent.Create(context.Message);
            var messageId = await SaveMessage(@event);

            if (messageId != 0)
            {
                @event.MessageClient.MessageId = messageId;
                await BroadcastToInitiator(@event);
            }
        }
        
        private async Task<long> SaveMessage(MessengerClientEvent eventContext)
        {
            var message = messenger.Create.Message
            (
                chatId:eventContext.Header.InformationChat.ChatId,
                accountId:eventContext.Header.Account.Id,
                messageTypeId:(int) Enum.Parse(eventContext.Header?.MessageType.GetType(),
                    eventContext.Header?.MessageType.ToString()),
                content:eventContext.MessageClient, 
                recipientId:eventContext.Header.Account.RecipientBotMessageId
            );

            var command = SaveMessageCommand.Create(message, eventContext.Header.Account.Email);
            var processingResult = await _mediator.Send(command);
            return processingResult.Result;
        }

        private async Task BroadcastToInitiator(MessengerClientEvent message)
        {
            var messages = new[] {message.MessageClient};

            var connectionId = message.Header.Account.ConnectionId;

            if (message.Header.MessageType == MessageType.Bot)
                await SendToInitiator(messages, connectionId);
            else
                await SendToChat(messages, message.Header.InformationChat.ChatId);
        }

        private Task SendToInitiator(MessageClient[] messages, string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return Task.CompletedTask;

            Log.Information($"******{nameof(SendToInitiator)} messages: {messages.ToJson()}");
            return _messenger
                .Clients
                .Client(connectionId)
                .SendAsync("onGetBotMessages", messages);
        }

        private async Task SendToChat(MessageClient[] messages, long chatId)
        {
            var chat = await _mediator.Send(GetChatByIdQuery.Create(chatId));
            
            var users = await _activeUsersService.GetUsersByEmail(chat.Accounts);
            var connectionIds = users.SelectMany(x => x.ConnectionIds).ToArray();
            
            await _messenger
                .Clients
                .Clients(connectionIds)
                .SendAsync("onGetMessages", messages, CancellationToken.None);
            
            Log.Information($"******{nameof(SendToChat)} messages: {messages.ToJson()}");
            await SendMessageToDisconnectedUsers(chatId, messages, chat.Accounts.ToArray());
        }

        private async Task SendMessageToDisconnectedUsers(long chatId, IEnumerable<MessageClient> messages,
            string[] accountsChat)
        {
            var activeUsers = await _activeUsersService.GetUsersByEmail(accountsChat);
            var activeUsersArray = activeUsers.Select(x => x.Login).ToArray();

            var disactiveUsers = accountsChat.Except(activeUsersArray);
            
            var accountIds = accountsChat
                .Where(x => disactiveUsers.Contains(x))
                .Select(x => x)
                .ToArray();

            if(disactiveUsers is null || disactiveUsers.Count() == 0)
                return;
            
            var query = GetAccountsChatByIdQuery.Create(chatId);
            var accountsId = await _mediator.Send(query);
            
            var @event = GoogleNotificationMessageEvent.Create
            (
                "New Message",
                JsonConvert.SerializeObject(messages),
                accountsId
            );
            await _bus.Publish(@event);
            
            Log.Information("GoogleNotificationMessageEvent");
            Log.Information($"accountIds: {accountIds.ToJson()}");
        }
    }
}