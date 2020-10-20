using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Events;
using SP.Messenger.Application.Accounts.Queries.GetAccountsByOrganizationIds;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MN = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands.BidCreateChat
{
    /// <summary>
    /// Команда на создание чата
    /// </summary>
    public class BidCreateChatConsumer : IConsumer<MessengerClientFromWorkflowtCreateChatEvent>
    {
        private readonly IMediator _mediator;
        public BidCreateChatConsumer(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task Consume(ConsumeContext<MessengerClientFromWorkflowtCreateChatEvent> context)
        {
            var accounts = await GetAccounts(context?.Message);

            var command = CreateChatCommand.Create(context?.Message?.ChatName,
                                    context?.Message?.ChatTypeMnemonic, 
                                    true, 
                                    context?.Message?.DocumentId ?? Guid.Empty,
                                    context?.Message?.DocumentTypeId ?? 0,
                                    context?.Message?.DocumentStatusMnemonic,
                                    (MN)context?.Message?.ModuleName,
                                    accounts,
                                    context?.Message?.Header?.InformationChat?.ParentDocumentId);

            var resultCommand = await _mediator.Send(command, CancellationToken.None);

            var response = BidCommonResponse.Create(context.Message?.DocumentId ?? Guid.Empty, resultCommand.Result);
            await context.RespondAsync(response);
        }

        private async Task<AccountMessengerDTO[]> GetAccounts(MessengerClientFromWorkflowtCreateChatEvent message)
        {
            var accounts = Array.Empty<AccountMessengerDTO>();
            if (message.Header.Account.OrganizationId.HasValue)
            {
                var query = GetAccountsByOrganizationIdsQuery.Create(
                    new[] { message.Header.Account.OrganizationId.Value },
                    message.ChatTypeMnemonic);
                accounts = await _mediator.Send(query);
            }
            else
            {
                accounts = message?.Accounts?.Select(x => new AccountMessengerDTO
                {
                    AccountId = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    Login = x.Email
                }).ToArray();
            }

            return accounts;
        }
    }
}
