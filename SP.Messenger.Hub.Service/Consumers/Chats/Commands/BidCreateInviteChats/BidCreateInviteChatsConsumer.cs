using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Accounts.Queries.GetAccountsByOrganizationIds;
using SP.Messenger.Application.Chats.Commands.AddPerformersToChatsInvite;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Common.Implementations;
using MN = SP.Consumers.Models.ModuleName;

namespace SP.Messenger.Hub.Service.Consumers.Chats.Commands.BidCreateInviteChats
{
    public class BidCreateInviteChatsConsumer : IConsumer<BidCreateInviteChatsRequest>
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _userService;
        public BidCreateInviteChatsConsumer(IMediator mediator, ICurrentUserService userService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        public async Task Consume(ConsumeContext<BidCreateInviteChatsRequest> context)
        {
            var currentUser = _userService.GetCurrentUser();

            var chats = context.Message.ChatPerformers;

            var resultCommands = new List<ProcessingResult<long>>();

            var organizationsList = chats.Select(x => x.PerformerId).ToList();

            if(currentUser.OrganizationId.HasValue)
                organizationsList.Add(currentUser.OrganizationId.Value);

            var accountListForExistChats = new List<AccountMessengerDTO>();
            //TODO: переделать модель и получит OrganizationId по Account
            foreach (var item in chats)
            {
                var performerIds = new List<long>();
                performerIds.Add(item.PerformerId);
                if (currentUser.OrganizationId.HasValue)
                {
                    performerIds.Add(currentUser.OrganizationId.Value);
                }
                //страшный кастыль, но надо сделать срочно
                var accounts = await GetAccounts
                (
                    performerIds.ToArray(),
                    chats.Select(x => x.ChatTypeMnemonic).First(),
                    context.CancellationToken
                );
                accountListForExistChats.AddRange(accounts);

                item.DocumentTypeId = 3;
                item.Module = "Bid";

                if (string.IsNullOrWhiteSpace(item.Name))
                    item.Name = $"Новый чат ({DateTimeOffset.Now})";

                var command = CreateChatCommand.Create(item.Name,
                    item.ChatTypeMnemonic,
                    true,
                    item.DocumentId,
                    item.DocumentTypeId,
                    item.DocumentStatusMnemonic,
                    Enum.Parse<MN>(item.Module),
                    accounts,
                    item.ParentDocumentId,
                    item.PerformerId);

                var resultCommand = await _mediator.Send(command, CancellationToken.None);
                resultCommands.Add(resultCommand);
            }

            var updateChatsCommand = new AddPerformersToChatsInviteCommand
            (
                accounts: accountListForExistChats,
                documentInviteId: chats.Select(x => x.ParentDocumentId).First()
            );
            await _mediator.Send(updateChatsCommand, context.CancellationToken);

            var errors = resultCommands.SelectMany(x => x.Errors);
            var response = BidCreateInviteChatsResponse.Create(true, errors.Select(x=>x.Message).ToArray());
            await context.RespondAsync(response);
        }

        private async Task<AccountMessengerDTO[]> GetAccounts(long[] organizationIds, string chatTypeMnemonic, CancellationToken token)
        {
            var query = GetAccountsByOrganizationIdsQuery.Create(organizationIds, chatTypeMnemonic);
            var accounts = await _mediator.Send(query, token);
            return accounts;
        }
    }
}