using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace SP.Messenger.Infrastructure.Services.Bids
{
    public class GetBidStatusService<TRequest, TResponse> : IGetBidStatusService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly IRequestClient<TRequest> _client;
        
        public GetBidStatusService(IRequestClient<TRequest> client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }
        
        public async Task<TResponse> GetBidStatus(TRequest request, CancellationToken cancellationToken)
        {
            var response = await _client.GetResponse<TResponse>(request, cancellationToken);
            return response?.Message;
        }
    }
}