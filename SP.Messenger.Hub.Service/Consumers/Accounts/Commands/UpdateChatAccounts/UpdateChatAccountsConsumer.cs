using System;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Chats.Commands.CreateChat.UpdateAccountsObject;

namespace SP.Messenger.Hub.Service.Consumers.Accounts.Commands.UpdateChatAccounts
{
    public class UpdateChatAccountsConsumer : IConsumer<UpdateChatAccountsRequest>
    {
        private readonly IMediator _mediator;

        public UpdateChatAccountsConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<UpdateChatAccountsRequest> context)
        {
            var command = UpdateAccountsObjectCommand.Create(context.Message.ProjectId);
            var result = await _mediator.Send(command);

            var response = UpdateChatAccountsResponse.Create(result);
            await context.RespondAsync(response);
        }
    }
}