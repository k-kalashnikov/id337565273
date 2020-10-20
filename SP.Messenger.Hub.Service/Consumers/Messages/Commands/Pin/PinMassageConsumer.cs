using System;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Messages.Command.Pined;

namespace SP.Messenger.Hub.Service.Consumers.Messages.Commands.Pin
{
    public class PinMassageConsumer : IConsumer<MessengerPinMessageEvent>
    {
        private readonly IMediator _mediator;

        public PinMassageConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        /// <summary>
        /// Пинит сообщение от ChatBot!!!
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<MessengerPinMessageEvent> context)
        {
            var command = PinMessageCommand.Create(context.Message.MessageId);
            _ = await _mediator.Send(command, context.CancellationToken);
        }
    }
}