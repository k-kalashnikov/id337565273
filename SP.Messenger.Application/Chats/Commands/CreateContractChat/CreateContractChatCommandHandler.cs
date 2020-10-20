using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Serilog;
using SP.Consumers.Models;
using SP.Market.Core.Extension;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Market.Identity.Common.Interfaces;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Common.Implementations;

namespace SP.Messenger.Application.Chats.Commands.CreateContractChat
{
    public class CreateContractChatCommandHandler : IRequestHandler<CreateContractChatCommand, ProcessingResult<long>>
    {
        private readonly Mediator _mediator;
        private readonly IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
        private readonly IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;
        private readonly ICurrentUserService _currentUserService;

        public CreateContractChatCommandHandler(ICurrentUserService currentUserService, 
            IRequestClient<GetUsersByRoleRequest> clientRoleRequest,
            IRequestClient<GetAccountsByOrganizationIdRequest> clientOrgRequest,
            Mediator mediator)
        {
            _currentUserService = currentUserService;
            _clientRoleRequest = clientRoleRequest;
            _clientOrgRequest = clientOrgRequest;
            _mediator = mediator;
        }
        public async Task<ProcessingResult<long>> Handle(CreateContractChatCommand request, CancellationToken cancellationToken)
        {
            var accounts = await GetAccountsAsync(request, cancellationToken);

            var command = CreateChatCommand.Create($"Договор №{request.Number}",
                "module.contract.chat.contract",
                true,
                request.Id,
                8,
                string.Empty, 
                ModuleName.Market,
                accounts,
                parentDocumentId:null,
                contractorId:null);

            return await _mediator.Send(command, cancellationToken);
        }
        
        private async Task<List<AccountMessengerDTO>> GetAccountsAsync(CreateContractChatCommand request, CancellationToken token)
        {
            var accounts = new List<AccountMessengerDTO>();
            
            var superUsers = await _clientRoleRequest
                .GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("superuser.module.platform"), token);
            
            accounts.AddRange(superUsers.Message.Accounts.Select(m => new AccountMessengerDTO
            {
                AccountId = m.AccountId,
                FirstName = m.FirstName,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                Login = m.Email
            }));

            var customersRequest = new GetAccountsByOrganizationIdRequest(request.CustomerOrganizationId);
            var usersCustomers = await _clientOrgRequest
                .GetResponse<GetAccountsByOrganizationIdResponse>(customersRequest, token);

            var categoryManagersRequest = new GetUsersByRoleRequest("manager.module.market");
            var usersCategoryManagers = await _clientRoleRequest
                .GetResponse<GetUsersByRoleResponse>(categoryManagersRequest, token);
            
            var currentUser = _currentUserService.GetCurrentUser();
            Log.Information($"*****-1 {nameof(CreateContractChatCommand)}");
            Log.Information($"*****-2 {nameof(currentUser)} : {currentUser.ToJson()}");
            Log.Information($"*****-3 {nameof(usersCategoryManagers)} : {usersCategoryManagers.ToJson()}");

            var categoryManagersOrgCurrentUser = usersCategoryManagers
                .Message.Accounts.Where(x => x.OrganizationId == currentUser.OrganizationId).ToList();
            
            Log.Information($"*****-4 {nameof(categoryManagersOrgCurrentUser)} : {categoryManagersOrgCurrentUser.ToJson()}");
            
            accounts.AddRange(categoryManagersOrgCurrentUser              
                .Select(m => new AccountMessengerDTO
                {
                    AccountId = m.AccountId,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    MiddleName = m.MiddleName,
                    Login = m.Email
                })
            );
            
            accounts.AddRange(
                usersCustomers.Message.Accounts
                    .Where(m => !accounts.Any(a => a.AccountId.Equals(m.Id)))
                    .Select(m => new AccountMessengerDTO
                    {
                        AccountId = m.Id,
                        FirstName = m.FirstName,
                        LastName = m.LastName,
                        MiddleName = m.MiddleName,
                        Login = m.Login
                    })
            );

            var contractorsRequest = new GetAccountsByOrganizationIdRequest(request.ContractorOrganizationId);
            var usersContractors = await _clientOrgRequest
                .GetResponse<GetAccountsByOrganizationIdResponse>(contractorsRequest, token);

            accounts.AddRange(
                usersContractors.Message.Accounts
              .Where(m => !accounts.Any(a => a.AccountId.Equals(m.Id)))
              .Select(m => new AccountMessengerDTO()
              {
                  AccountId = m.Id,
                  FirstName = m.FirstName,
                  LastName = m.LastName,
                  MiddleName = m.MiddleName,
                  Login = m.Login
              })
            );
            return accounts;
        }
    }
}