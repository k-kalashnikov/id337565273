using MassTransit;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SP.Market.EventBus.RMQ.Shared.Events.Messenger;
using SP.Messenger.Application.Chats.Commands.OrganizationChatInit;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands
{
    public class OrganizationChatsInit : IConsumer<OrgnizationChatInitEvent>
    {
        private readonly IMediator _mediator;

        public OrganizationChatsInit(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<OrgnizationChatInitEvent> context)
        {
            var request = context.Message;

            var command = OrganizationChatInitCommand.Create(context.Message.Accounts, context.Message.Organizations);

            var resultCommand = await _mediator.Send(command, context.CancellationToken);

            await context.RespondAsync(resultCommand.Result);
        }
    }
}
