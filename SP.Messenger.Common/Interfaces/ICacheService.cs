using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Application.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken);
        Task SetAsync<T>(string key, T data, double AbsoluteExpiration = 6, double SlidingExpiration = 15, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}
