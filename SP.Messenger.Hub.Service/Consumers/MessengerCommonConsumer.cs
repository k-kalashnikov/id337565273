using MassTransit;
using MediatR;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.MessengerAssistent.Commands;
using System;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers
{
    public class MessengerCommonConsumer : IConsumer<MessengerServerEvent>
    {
        private readonly IMediator _mediator;
        public MessengerCommonConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<MessengerServerEvent> context)
        {
            if (context?.Message?.Header?.MessageType != MessageType.Bot)
            {
                var command = MessengerAssistentCommand.Create(context?.Message);
                await _mediator.Send(command);
            }
        }
    }
}
