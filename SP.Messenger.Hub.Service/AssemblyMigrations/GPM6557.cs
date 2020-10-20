using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using SP.Consumers.Models;
using SP.Contract.Client.Interfaces;
using SP.Contract.Events.Event.CreateContract;
using SP.Market.EventBus.RMQ.Shared.Models.Platform.Accounts.GetUsersByRole;
using SP.Messenger.Application.Chats.Commands.CreateChat;
using SP.Messenger.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Hub.Service.AssemblyMigrations
{
	public class GPM6557 : IAssemblyMigration
	{
        private IContractClientService _contractClientService;
        private IMediator _mediator;
        private IRequestClient<GetAccountsByOrganizationIdRequest> _clientOrgRequest;
        private IRequestClient<GetUsersByRoleRequest> _clientRoleRequest;
        private MessengerDbContext _context;

        public IServiceProvider ServiceProvider { get; set; }

		public GPM6557(IServiceProvider serviceProvider) 
        {
            ServiceProvider = serviceProvider;
        }

        public async Task DoWork(CancellationToken cancellationToken)
		{
            Log.Information($"{nameof(GPM6557)} invoked");


            using (var scope = ServiceProvider.CreateScope())
            {
                _contractClientService = scope.ServiceProvider.GetRequiredService<IContractClientService>();
                _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                _clientOrgRequest = scope.ServiceProvider.GetRequiredService<IRequestClient<GetAccountsByOrganizationIdRequest>>();
                _clientRoleRequest = scope.ServiceProvider.GetRequiredService<IRequestClient<GetUsersByRoleRequest>>();
                _context = scope.ServiceProvider.GetRequiredService<MessengerDbContext>();

                if ((_contractClientService == null)
                    || (_mediator == null)
                    || (_clientOrgRequest == null)
                    || (_clientRoleRequest == null)
                    || (_context == null)
                    )
                {
                    Log.Error($"{nameof(GPM6557)} Required Service not scoped");
                    return;
                }


                var contracts = await _contractClientService.GetContractsAsync();
                var documents = await _context.Documents.ToListAsync();

                var result = contracts
                    .Where(c => !documents.Any(d => d.DocumentId.ToString().Equals(c.Id)))
                    .ToList();

				foreach (var item in result)
				{
                    await CreateChat(new CreateContractEvent()
                    {
                        Id = Guid.Parse(item.Id),
                        Number = item.Number,
                        ContractorOrganizationId = item.ContractorOrganizationId,
                        CustomerOrganizationId = item.CustomerOrganizationId
                    }, cancellationToken);
                }
            }


            Log.Information($"{nameof(GPM6557)} complited");
        }


        public async Task CreateChat(CreateContractEvent request, CancellationToken cancellationToken)
        {
            Log.Information($"{nameof(GPM6557)} invoked");
            var accounts = await GetAccountsAsync(request, cancellationToken);

            accounts.ForEach(m =>
            {
                Log.Information($"{nameof(GPM6557)} account = {m.AccountId}; login={m.Login}");
            });

            var command = CreateChatCommand.Create($"Договор №{request.Number}",
                    "module.contract.chat.contract",
                    true,
                    request.Id,
                    8,
                    "",
                    SP.Consumers.Models.ModuleName.Market,
                    accounts,
                    null,
                    null);

            var resultCommand = await _mediator.Send(command, cancellationToken);

            var response = new ChatInfoResponse
            {
                ChatName = $"Договор №{request.Number}",
                ChatId = resultCommand.Result,
                DocumentId = request.Id,
                ParentDocumentId = null,
                ChatTypeMnemonic = "module.contract.chat.contract"
            };

            Log.Information($"{nameof(GPM6557)} complited ChatId = {response.ChatId} ChatName={response.ChatName}");
        }


        public async Task<List<AccountMessengerDTO>> GetAccountsAsync(CreateContractEvent request, CancellationToken cancellationToken)
        {
            Log.Information($"{nameof(GPM6557)}:GetAccountsAsync(context)");
            Log.Information($"{nameof(GPM6557)}:context:{JsonConvert.SerializeObject(request)}");

            var accounts = new List<AccountMessengerDTO>();

            Log.Information($"{nameof(GPM6557)} GetUsersByRoleRequest (superuser.module.platform)");
            var responseByRole = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("superuser.module.platform") { }, cancellationToken);
            Log.Information($"{nameof(GPM6557)} GetUsers: {JsonConvert.SerializeObject(responseByRole)}");
            accounts.AddRange(responseByRole.Message.Accounts.Select(m => new AccountMessengerDTO()
            {
                AccountId = m.AccountId,
                FirstName = m.FirstName,
                LastName = m.LastName,
                MiddleName = m.MiddleName,
                Login = m.Email
            }));

            Log.Information($"{nameof(GPM6557)} GetAccountsByOrganizationIdRequest(customerOrganizationId:{request.CustomerOrganizationId})");
            var responseByOrg = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(request.CustomerOrganizationId), cancellationToken);
            Log.Information($"{nameof(GPM6557)} GetUsers: {JsonConvert.SerializeObject(responseByOrg)}");

            Log.Information($"{nameof(GPM6557)} GetUsersByRoleRequest(manager.module.market)");
            var responseByRoleKM = await _clientRoleRequest.GetResponse<GetUsersByRoleResponse>(new GetUsersByRoleRequest("manager.module.market") { }, cancellationToken);
            Log.Information($"{nameof(GPM6557)} GetUsers: {JsonConvert.SerializeObject(responseByRoleKM)}");

            accounts.AddRange(
              responseByOrg.Message.Accounts
              .Where(m => responseByRoleKM.Message.Accounts.Any(r => r.AccountId.Equals(m.Id)))
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

            Log.Information($"{nameof(GPM6557)} GetAccountsByOrganizationIdRequest(contractorOrganizationId:{request.ContractorOrganizationId})");
            var responseByPer = await _clientOrgRequest.GetResponse<GetAccountsByOrganizationIdResponse>(new GetAccountsByOrganizationIdRequest(request.ContractorOrganizationId), cancellationToken);
            Log.Information($"{nameof(GPM6557)} GetUsers: {JsonConvert.SerializeObject(responseByPer)}");

            accounts.AddRange(
              responseByPer.Message.Accounts
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

            Log.Information($"{nameof(GPM6557)} All users: {JsonConvert.SerializeObject(accounts)}");

            return accounts;
        }
    }
}
