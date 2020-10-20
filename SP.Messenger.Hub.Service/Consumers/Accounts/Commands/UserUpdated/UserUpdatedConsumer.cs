using MassTransit;
using MediatR;
using SP.Messenger.Application.Chats.Commands.AddExistAccountToExistChats;
using SP.Messenger.Application.Chats.Commands.AddNewAccountsToExistsChats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.Consumers.Accounts.Commands.UserUpdated
{
    public class UserUpdatedConsumer : IConsumer<Market.EventBus.RMQ.Shared.Events.Users.UserCreatedEvent>
    {
        private readonly IMediator _mediator;

        public UserUpdatedConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Consume(ConsumeContext<Market.EventBus.RMQ.Shared.Events.Users.UserCreatedEvent> context)
        {
            var command = AddExistAccountToExistChatsCommand.Create(
                context.Message.AccountId,
                context.Message.Roles);

            await _mediator.Send(command, context.CancellationToken);
        }
    }
}
