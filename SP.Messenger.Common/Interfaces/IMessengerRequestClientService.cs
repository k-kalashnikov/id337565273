using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Common.Interfaces
{
    public interface IMessengerRequestClientService<TRequest, TResponse>
    {
        Task<TResponse> GetResponseAsync(TRequest request, CancellationToken token);
    }
}
