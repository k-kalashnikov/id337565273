using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SP.Consumers.Models;
using SP.Messenger.Hub.Service.Hub;

namespace SP.Messenger.Hub.Service.Consumers.PanelButtons
{
    public class PanelButtonCommandsConsumer : IConsumer<PanelButtonCommandsRequest>
    {
        private readonly IHubContext<MessengerHub> _messenger;

        public PanelButtonCommandsConsumer(IHubContext<MessengerHub> messenger)
        {
            _messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        }


        public async Task Consume(ConsumeContext<PanelButtonCommandsRequest> context)
        {
            await SendToInitiator(context.Message.ChatButtons, context.Message.ConnectionId);  
            await context.RespondAsync(PanelButtonCommandsResponse.Create(true));
        }
        
        private async Task SendToInitiator(IEnumerable<ButtonCommand> buttons, string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
                return;

            await _messenger
                .Clients
                .Client(connectionId)
                .SendAsync("onGetPanelButtons", buttons);
        }
    }
}