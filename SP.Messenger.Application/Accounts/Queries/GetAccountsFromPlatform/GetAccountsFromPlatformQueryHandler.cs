using MediatR;
using SP.Consumers.Models;
using SP.Messenger.Infrastructure.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Accounts.Queries.GetAccountsFromPlatform
{
    public class GetAccountsFromPlatformQueryHandler
        : IRequestHandler<GetAccountsFromPlatformQuery, AccountMessengerDTO[]>
    {
        private readonly IAccountPlatformService<RequestAccountsByOrganization, AccountForMessengerDTO[]> _client;

        public GetAccountsFromPlatformQueryHandler(IAccountPlatformService<RequestAccountsByOrganization, AccountForMessengerDTO[]> client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<AccountMessengerDTO[]> Handle(GetAccountsFromPlatformQuery request, CancellationToken cancellationToken)
        {
            var requestBody = new RequestAccountsByOrganization(request.OrganizationId);
            var response = await _client.GetAccount(requestBody, cancellationToken);

            return AccountMessengerDTO.Create(response);
        }
    }
}
