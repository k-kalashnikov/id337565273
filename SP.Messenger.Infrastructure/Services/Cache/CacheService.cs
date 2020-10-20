using Microsoft.Extensions.Caching.Distributed;
using SP.Market.Core.Extension;
using SP.Messenger.Application.Interfaces;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SP.Messenger.Infrastructure.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken)
        {
            T data = default;

            var bytes = await _cache.GetAsync(key, cancellationToken);

            if (bytes != null)
            {
                string stringData = Encoding.UTF8.GetString(bytes);
                data = stringData.FromJson<T>();
            }

            return data;
        }

        public async Task SetAsync<T>(string key, T data, double AbsoluteExpiration = 60, double SlidingExpiration = 150, CancellationToken cancellationToken = default)
        {
            var dataString = data.ToJson();

            var bytes = Encoding.UTF8.GetBytes(dataString);

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(AbsoluteExpiration))
                .SetSlidingExpiration(TimeSpan.FromMinutes(SlidingExpiration));

            await _cache.SetAsync(key, bytes, options, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
