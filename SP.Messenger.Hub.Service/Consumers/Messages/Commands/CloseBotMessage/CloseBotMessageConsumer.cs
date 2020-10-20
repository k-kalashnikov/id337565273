using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Messages.Command.CloseBotMessage;
using System;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers.Messages.Commands.CloseBotMessage
{
    public class CloseBotMessageConsumer : IConsumer<CloseBotMessageRequest>
    {
        private readonly IMediator _mediator;
        public CloseBotMessageConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<CloseBotMessageRequest> context)
        {
            var command = CloseBotMessageCommand.Create(context.Message.MessageId,
                context.Message.Value, context.Message.Content);

            _ = await _mediator.Send(command);

            await context.RespondAsync(CloseBotMessageResponse.Create(true));
        }
    }
}