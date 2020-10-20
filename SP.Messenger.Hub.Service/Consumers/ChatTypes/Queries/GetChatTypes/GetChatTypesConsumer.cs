using System;
using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Queries.GetChatType;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers.ChatTypes.Queries.GetChatTypes
{
    public class GetChatTypesConsumer : IConsumer<GetChatTypeRequest>
    {
        private readonly IMediator _mediator;
        public GetChatTypesConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<GetChatTypeRequest> context)
        {
            var chatTypes = await _mediator.Send(GetChatTypeByChatQuery.Create(context.Message.ChatId));
            await context.RespondAsync(ChatTypeResponse.Create(chatTypes));
        }
    }
}
