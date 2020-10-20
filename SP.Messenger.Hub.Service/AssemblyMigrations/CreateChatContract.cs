using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SP.Consumers.Models;
using SP.Contract.Client.Interfaces;
using SP.Contract.Client.Models;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Application.Chats.Commands.CreateContractChat;
using SP.Messenger.Common.Extensions;
using SP.Messenger.Persistence;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
    public class CreateChatContract : AssemblyMigration
    {
        private const string ChatMnemonic = "module.contract.chat.contract";
        
        private IContractClientService _contractClientService;
        private Mediator _mediator;
        private IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
        private IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;
        private IRequestClient<GetAccountIdentityRequest> _clientAccountRequest;

        public CreateChatContract(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = ServiceProvider.CreateScope();
            _contractClientService = scope.ServiceProvider.GetService<IContractClientService>();
            _mediator = scope.ServiceProvider.GetService<Mediator>();
            
            _clientOrgRequest = scope.ServiceProvider.GetService<IRequestClient<GetAccountsByOrganizationIdRequest>>();
            _clientRoleRequest = scope.ServiceProvider.GetService<IRequestClient<GetUsersByRoleRequest>>();
            _clientAccountRequest = scope.ServiceProvider.GetService<IRequestClient<GetAccountIdentityRequest>>();
            
            var context = scope.ServiceProvider.GetService<MessengerDbContext>();
            
            var contracts = await _contractClientService.GetContractsAsync(cancellationToken);

            var contractChats = await context.ChatView
                .Where(x => x.Mnemonic == ChatMnemonic)
                .ToArrayAsync(cancellationToken);

            foreach (var contract in contracts)
            {
                var chat = contractChats.FirstOrDefault(x => x.DocumentId == contract.Id);
                if(chat != null)
                    continue;

                await CreateChat(contract, context, cancellationToken);
            }
            
        }

        private async Task CreateChat(ContractDto contract, MessengerDbContext context, CancellationToken cancellationToken)
        {
            var accounts = await GetAccountsAsync(contract, context, cancellationToken);
            var command = CreateChatCommand.Create($"Договор №{contract.Number}",
                "module.contract.chat.contract",
                true,
                Guid.Parse(contract.Id),
                8,
                string.Empty, 
                ModuleName.Market,
                accounts,
                parentDocumentId:null,
                contractorId:null);

            await _mediator.Send(command, cancellationToken);
        }
        
        private async Task<List<AccountMessengerDTO>> GetAccountsAsync(ContractDto contract, MessengerDbContext context, CancellationToken token)
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

            var customersRequest = new GetAccountsByOrganizationIdRequest(contract.CustomerOrganizationId);
            var usersCustomers = await _clientOrgRequest
                .GetResponse<GetAccountsByOrganizationIdResponse>(customersRequest, token);

            var categoryManagersRequest = new GetUsersByRoleRequest("manager.module.market");
            var usersCategoryManagers = await _clientRoleRequest
                .GetResponse<GetUsersByRoleResponse>(categoryManagersRequest, token);

            var accountDb = await context.Accounts
                .FirstOrDefaultAsync(x=>x.AccountId == contract.CreatedBy.Id, token);
            
            var identityRequest = GetAccountIdentityRequest.Create(accountDb.Login);
            var resposnse = await _clientAccountRequest.GetResponse<GetAccountIdentityResponse>(identityRequest, token);
            var account = resposnse.Message;
            
            Log.Information($"*****-1 {nameof(CreateContractChatCommand)}");
            Log.Information($"*****-2 {nameof(account)} : {account.ToJson()}");
            Log.Information($"*****-3 {nameof(usersCategoryManagers)} : {usersCategoryManagers.ToJson()}");

            var categoryManagersOrgCurrentUser = usersCategoryManagers
                .Message.Accounts.Where(x => x.OrganizationId == account.OrganizationId).ToList();
            
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

            var contractorsRequest = new GetAccountsByOrganizationIdRequest(contract.ContractorOrganizationId);
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