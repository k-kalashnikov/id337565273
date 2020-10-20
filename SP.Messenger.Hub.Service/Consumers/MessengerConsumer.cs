using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Serilog;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Accounts.Queries.GetAccountsChatById;
using SP.Messenger.Application.Chats.Queries.GetChatById;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Hub.Service.Hub;
using SP.Messenger.Hub.Service.Services;

namespace SP.Messenger.Hub.Service.Consumers
{
    public class MessengerConsumer : IConsumer<MessengerClientEvent>
    {
        private readonly IHubContext<MessengerHub> _messenger;
        private readonly IActiveUsersService _activeUsersService;
        private readonly IMediator _mediator;
        private readonly IBusControl _bus;

        public MessengerConsumer(IHubContext<MessengerHub> messenger,
            IActiveUsersService activeUsersService, IMediator mediator, IBusControl bus)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
            _activeUsersService = activeUsersService ?? throw new ArgumentNullException(nameof(activeUsersService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }
        
        public async Task Consume(ConsumeContext<MessengerClientEvent> context)
        {
            await BroadcastToInitiator(context.Message);
        }
        
        private async Task BroadcastToInitiator(MessengerClientEvent message)
        {
            var messages = new [] { message.MessageClient };

            var connectionId = message.Header.Account.ConnectionId;

            if (message.Header.MessageType == MessageType.Bot)
                await SendToInitiator(messages, connectionId);
            else
                await SendToChat(messages, message.Header.InformationChat.ChatId);
        }

        private Task SendToInitiator(MessageClient[] messages, string connectionId)
        {
            Log.Information($"******{nameof(SendToInitiator)} messages: {messages.ToJson()}");
            return _messenger
                .Clients
                .Client(connectionId)
                .SendAsync("onGetBotMessages", messages);
        }

        private async Task SendToChat(MessageClient[] messages, long chatId)
        {
            Log.Information($"******{nameof(SendToChat)} chatId: {chatId}");
            var chat = await _mediator.Send(GetChatByIdQuery.Create(chatId));
            Log.Information($"******{nameof(SendToChat)} recipiends: {chat.Accounts.ToJson()}");
            var users = await _activeUsersService.GetUsersByEmail(chat.Accounts);
            var connectedIds = users.SelectMany(x => x.ConnectionIds).ToArray();
            Log.Information($"******{nameof(SendToChat)} connectionIds: {connectedIds.ToJson()}");
            await _messenger
                .Clients
                .Clients(connectedIds)
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
            
            if(disactiveUsers is null || disactiveUsers.Count() == 0)
                return;
            
            var accountIds = accountsChat
                .Where(x => disactiveUsers.Contains(x))
                .Select(x => x)
                .ToArray();

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