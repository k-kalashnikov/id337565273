using System;
using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.Accounts.Queries.GetAccountsChat;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using SP.Market.Core.Extension;

namespace SP.Messenger.Hub.Service.Consumers.Accounts.Queries.GetAccounts
{
    public class GetAccountsChatConsumer : IConsumer<RequestChat>
    {
        private readonly IMediator _mediator;
        public GetAccountsChatConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<RequestChat> context)
        {
            var command = GetAccountByChat.Create(context.Message.ChatId, context.Message.OrganizationId);
            var accountsPlatform = await _mediator.Send(command);
            
            Log.Information($"GetAccountsChatConsumer. accounts: {accountsPlatform.ToJson()}");
            
            var accounts = accountsPlatform?.Select(x => 
                AccountsChatResponse.Create(x.AccountId, x.Login, x.FirstName, x.LastName, x.MiddleName))
                .ToArray();

            Log.Information($"GetAccountsChatConsumer. RespondAsync: {accounts?.ToJson()}");
            await context.RespondAsync(accounts);
        }
    }
}
