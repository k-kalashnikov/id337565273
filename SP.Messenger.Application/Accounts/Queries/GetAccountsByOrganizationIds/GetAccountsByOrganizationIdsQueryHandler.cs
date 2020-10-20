using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Application.ChatTypeRoles.Queries;
using SP.Messenger.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsByOrganizationIds
{
    public class GetAccountsByOrganizationIdsQueryHandler : IRequestHandler<GetAccountsByOrganizationIdsQuery, AccountMessengerDTO[]>
    {
        private readonly IMediator _mediator;
        private readonly IAccountIdentityService<GetAccountsByRolesRequest, GetAccountsByRolesResponse[]> _identityService;

        public GetAccountsByOrganizationIdsQueryHandler(IMediator mediator,
            IAccountIdentityService<GetAccountsByRolesRequest, GetAccountsByRolesResponse[]> identityService)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        public async Task<AccountMessengerDTO[]> Handle(GetAccountsByOrganizationIdsQuery request, CancellationToken cancellationToken)
        {
            var accounts = new List<AccountMessengerDTO>();

            var roles = await _mediator.Send(GetRolesQuery.Create(request.ChatTypeMnemonic));

            var accountsResponse = await GetAccounts(request.OrganizationIds, roles.Roles, cancellationToken);

            foreach (var item in accountsResponse)
            {
                if (accounts.FirstOrDefault(x => x.AccountId == item.AccountId) is null)
                {
                    accounts.Add(new AccountMessengerDTO
                    {
                        AccountId = item.AccountId,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        Login = item.Login,
                        MiddleName = item.MiddleName
                    });
                }
            }

            return accounts.ToArray();
        }

        private async Task<GetAccountsByRolesResponse[]> GetAccounts(long[] organizationIds, string[] roles, CancellationToken cancellationToken)
        {
            var organizations = new List<GetAccountsByRolesRequest.OrganizationRoles>();
            foreach (var item in organizationIds)
            {
                organizations.Add(new GetAccountsByRolesRequest.OrganizationRoles
                {
                    OrganizationId = item,
                    Roles = roles
                });
            }

            var request = new GetAccountsByRolesRequest(true, organizations);
            var accountResponse = await _identityService.GetResponseAsync(request, cancellationToken);
            return accountResponse;
        }
    }
}