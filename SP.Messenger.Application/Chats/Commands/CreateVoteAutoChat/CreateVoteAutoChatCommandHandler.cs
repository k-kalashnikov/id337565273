using MassTransit;
using MediatR;
using SP.Consumers.Models;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Purchases.GetPurchases;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Accounts.Queries.GetAccountsByIds;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Application.Messages.Command;
using SP.Messenger.Application.Voting.Commands.CreateVoting;
using SP.Messenger.Common.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using messenger = SP.Messenger.Application.Messages.Builders;

namespace SP.Messenger.Application.Chats.Commands.CreateVoteAutoChat
{
    public class CreateVoteAutoChatCommandHandler : IRequestHandler<CreateVoteAutoChatCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;
        private readonly IRequestClient<GetPurchasesRequest> _clientPurchaseRequest;

        public CreateVoteAutoChatCommandHandler(IMediator mediator, 
            ICurrentUserService currentUserService,
            IRequestClient<GetUsersByRoleRequest> clientRoleRequest,
            IRequestClient<GetPurchasesRequest> clientPurchaseRequest)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
            _clientPurchaseRequest = clientPurchaseRequest;
            _clientRoleRequest = clientRoleRequest;
        }

        public async Task<bool> Handle(CreateVoteAutoChatCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            var resultChat = await CreateChat(request, currentUser, cancellationToken);

            var resultVote = await CreateVote(request, cancellationToken);

            var operationResult = await SaveMessage(request, resultChat.Result,
            resultVote.Result, currentUser, cancellationToken);

            return operationResult.Result != 0;
        }

        private async Task<ProcessingResult<long>> CreateChat(CreateVoteAutoChatCommand data, ICurrentUser currentUser,
            CancellationToken token)
        {

            var accounts = new List<long>();
            accounts.AddRange(data.Accounts);
            accounts.Add(currentUser.Id);

            var usersByRoleRequest = new GetUsersByRoleRequest("superuser.module.platform");
            var usersResponse = await _clientRoleRequest
                .GetResponse<GetUsersByRoleResponse>(usersByRoleRequest, token);

            if (usersResponse.Message.Accounts.Any())
            {
                var accountIds = usersResponse.Message.Accounts.Select(x => x.AccountId);
                accounts.AddRange(accountIds);
            }

            if (data.ParentDocumentId.HasValue && data.ParentDocumentId.Value != default)
            {
                var purchaseRequest = new GetPurchasesRequest(new [] {data.ParentDocumentId.Value});
                var purchases = await _clientPurchaseRequest
                    .GetResponse<GetPurchasesResponse>(purchaseRequest, token);

                var purchase = purchases.Message.Purchases?.FirstOrDefault();
                if (purchase != null)
                {
                    accounts.Add(purchase.ResponsiblePerson);
                }
            }


            //if (currentUser.OrganizationId.HasValue)
            //{
            //	Log.Information($"{nameof(CreateVoteAutoChatCommandHandler)} organization id of current user = {currentUser.OrganizationId.Value}");

            //	var responseByOrg = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(currentUser.OrganizationId.Value), token);
            //	Log.Information($"{nameof(CreateVoteAutoChatCommandHandler)} get {responseByOrg.Message.Accounts.Length} users of organization {currentUser.OrganizationId}");

            //	var responseByRoleKM = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("manager.module.market") { }, token);
            //	accounts.AddRange(responseByOrg.Message.Accounts
            //		.Where(m => responseByRoleKM.Message.Accounts.Any(r => r.AccountId.Equals(m.Id)))
            //		.Select(m => m.Id));
            //}

            var accountsByIdsRequest = GetAccountsByIdsQuery.Create(accounts.Distinct());
            var accountMessengers = await _mediator.Send(accountsByIdsRequest, token);

            var createChatCommand = CreateChatCommand.Create(data.Name,
                chatTypeMnemonic: "module.market.chat.vote",
                isActive: true,
                documentId: data.DocumentId,
                parentDocumentId: data.ParentDocumentId,
                documentTypeId: 1,
                documentStatusMnemonic: string.Empty,
                moduleName: ModuleName.Market,
                accounts: accountMessengers
                );

            return await _mediator.Send(createChatCommand, token);
        }

        private async Task<ProcessingResult<Guid>> CreateVote(CreateVoteAutoChatCommand data, CancellationToken token)
        {
            var variants = new List<ResponseVariantCommand>();

            foreach (var item in data.ResponseVariants)
            {
                variants.Add(new ResponseVariantCommand(
                    item.VariantName,
                    item.DecisionId,
                    item.Contractors.Select(x => new Voting.Commands.CreateVoting.Contractor
                    {
                        ContractorId = x.ContractorId,
                        ContractorName = x.ContractorName,
                        PriceOffer = x.PriceOffer,
                        DeviationBestPrice = x.DeviationBestPrice,
                        Term = x.Term,
                        TermDeviation = x.TermDeviation,
                        DefermentPayment = x.DefermentPayment,
                        DefermentDeviation = x.DefermentDeviation,
                        PercentDifferentByPurchase = x.PercentDifferentByPurchase,
                        PercentDifferentByBestContractorOffer = x.PercentDifferentByBestContractorOffer,
                        TermLimit = x.TermLimit,
                        ProtocolNumber = data.ProtocolNumber
                    })));
            }

            var command = new CreateVotingAutoCommand(data.Name, variants, data.Accounts);
            var result = await _mediator.Send(command, token);
            return result;
        }

        private async Task<ProcessingResult<long>> SaveMessage(CreateVoteAutoChatCommand data, long chatId,
            Guid votingId, ICurrentUser currentUser, CancellationToken token)
        {
            var accountsByIdsRequest = GetAccountsByIdsQuery.Create(data.Accounts);
            var accounts = await _mediator.Send(accountsByIdsRequest, token);

            var messageClient = new MessageClient
            {
                Author = new Author
                (
                    currentUser.Id,
                    currentUser.Login,
                    currentUser.FirstName,
                    currentUser.LastName,
                    currentUser.MiddleName
                ),
                ButtonCommands = Array.Empty<ButtonCommand>(),
                ChatId = chatId,
                ChatTypeMnemonic = "module.market.chat.vote",
                Commands = Array.Empty<CommandClient>(),
                Date = DateTime.UtcNow,
                DocumentId = data.DocumentId,
                Edited = false,
                File = null,
                Pined = false,
                Readed = false,
                ModuleName = ModuleName.Market,
                MessageId = 1,
                MessageType = MessageTypeClient.User,
                Content = string.Empty,
                VotingClient = new VotingClient
                {
                    VotingId = votingId,
                    Name = data.Name,
                    VotingObjects = data.ResponseVariants.Select(x => new VotingObject
                    {
                        Name = x.VariantName,
                        Contractors = x.Contractors.Select(c => new VotingContractor
                        {
                            ContractorId = c.ContractorId,
                            ContractorName = c.ContractorName,
                            PriceOffer = c.PriceOffer,
                            DeviationBestPrice = c.DeviationBestPrice,
                            Term = c.Term,
                            TermDeviation = c.TermDeviation,
                            DefermentPayment = c.DefermentPayment,
                            DefermentDeviation = c.DefermentDeviation,
                            PercentDifferentByPurchase = c.PercentDifferentByPurchase,
                            PercentDifferentByBestContractorOffer = c.PercentDifferentByBestContractorOffer,
                            TermLimit = c.TermLimit
                        }),
                        Accounts = accounts?.Select(i => new VotedAccounts
                        {
                            AccountId = i.AccountId,
                            Email = i.Login,
                            FirstName = i.FirstName,
                            LastName = i.LastName,
                            MiddleName = i.MiddleName
                        }).ToArray()
                    }).ToArray()
                }
            };
            
            var message = messenger.Create.Message
            (
                chatId: chatId,
                accountId: currentUser.Id,
                messageTypeId: 1,
                content: messageClient
            );

            var command = SaveMessageCommand.Create(message, currentUser.Login);
            return await _mediator.Send(command, token);
        }
    }
}
