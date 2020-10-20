using MassTransit;
using MediatR;
using SP.Messenger.Application.Messages.Command.AttachedFileProtocolFromService;
using SP.Protocol.Events.Common.Event.Protocol.CompletedProtocolReport;
using System;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers.CompletedProtocol
{
    public class CompletedProtocolReportConsumer : IConsumer<CompletedProtocolReportEvent>
    {
        private readonly IMediator _mediator;

        public CompletedProtocolReportConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<CompletedProtocolReportEvent> context)
        {
            var command = new AttachedFileProtocolFromServiceCommand
            (
                protocolId: context.Message.ProtocolId,
                link: context.Message.Link
            );
            await _mediator.Send(command, context.CancellationToken);
        }
    }
}
