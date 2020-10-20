using MassTransit;
using SP.Messenger.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services
{
    public class MessengerRequestClientService<TRequest, TResponse> : IMessengerRequestClientService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        private readonly IRequestClient<TRequest> _client;

        public async Task<TResponse> GetResponseAsync(TRequest request, CancellationToken token)
        {
            var response = await _client.GetResponse<TResponse>(request, token);
            return response.Message;
        }
    }
}
