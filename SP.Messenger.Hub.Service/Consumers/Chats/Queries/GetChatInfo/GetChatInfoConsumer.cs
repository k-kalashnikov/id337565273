using System;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Queries.GetChatInfo;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Queries.GetChatInfo
{
    public class GetChatInfoConsumer : IConsumer<GetChatInfoRequest>
    {
        private readonly IMediator _mediator;

        public GetChatInfoConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<GetChatInfoRequest> context)
        {
            var query = GetChatInfoQuery.Create(context.Message.AccountId,
                context.Message.DocumentId, context.Message.ContractorId);
            
            var result = await _mediator.Send(query);
            var response = GetChatInfoResponse.Create(result);
            
            await context.RespondAsync(response);
        }
    }
}