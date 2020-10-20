using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services
{
    public interface IAccountPlatformService<TRequest, TResponse>
    {
        Task<TResponse> GetAccount(TRequest request, CancellationToken cancellationToken);
    }
}