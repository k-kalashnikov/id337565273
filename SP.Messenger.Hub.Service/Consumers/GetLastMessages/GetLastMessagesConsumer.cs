using System;
using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Queries.GetLastMessage;
using System.Threading.Tasks;
using SP.Messenger.Common.Extensions;

namespace SP.Messenger.Hub.Service.Consumers.GetLastMessages
{
    public class GetLastMessagesConsumer : IConsumer<GetLastMessagesRequest>
    {
        private readonly IMediator _mediator;
        public GetLastMessagesConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<GetLastMessagesRequest> context)
        {
            var command = GetLastMessageQuery.Create(context.Message.StockOrderId);
            var chats = await _mediator.Send(command);
            
            var response = GetLastMessagesResponse.Create(chats.ToJson());
            await context.RespondAsync(response);
        }
    }
}
