using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services
{
    public interface IAccountIdentityService<TRequest, TResponse>
    {
        Task<TResponse> GetResponseAsync(TRequest request, CancellationToken cancellationToken);
    }
}