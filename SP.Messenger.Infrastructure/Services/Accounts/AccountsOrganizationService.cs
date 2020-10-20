using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace SP.Messenger.Infrastructure.Services.Accounts
{
    public class AccountsOrganizationService<TRequest, TResponse> : IAccountsOrganizationService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly IRequestClient<TRequest> _client;
        public AccountsOrganizationService(IRequestClient<TRequest> client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<TResponse> GetAccountByOrganizationIds(TRequest request, CancellationToken cancellationToken)
        {
            var response = await _client.GetResponse<TResponse>(request, cancellationToken);
            return response?.Message;
        }
    }
}