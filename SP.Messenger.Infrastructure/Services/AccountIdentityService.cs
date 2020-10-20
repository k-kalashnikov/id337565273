using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace SP.Messenger.Infrastructure.Services
{
    public class AccountIdentityService<TRequest, TResponse> : IAccountIdentityService<TRequest, TResponse>
        where TRequest : class
        where TResponse: class
    {
        private readonly IRequestClient<TRequest> _client;
        public AccountIdentityService(IRequestClient<TRequest> client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        public async Task<TResponse> GetResponseAsync(TRequest request, CancellationToken cancellationToken)
        {
            var response = await _client.GetResponse<TResponse>(request, cancellationToken);
            return response.Message;
        }
    }
}