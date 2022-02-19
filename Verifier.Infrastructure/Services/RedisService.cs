using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Verifier.Application.Interfaces.Services;
using Verifier.Shared.Enums;

namespace Verifier.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDistributedCache _distributedCache;
        public RedisService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<string> GetAsync(string key)
        {
            return await _distributedCache.GetStringAsync(key);
        }

        public async Task SaveAsync(string key, string data, int durationInMins, RedisCacheOptions? option)
        {
            var options = new DistributedCacheEntryOptions();

            if (option != null && option == RedisCacheOptions.SlidingExpiration)
                options.SetSlidingExpiration(new TimeSpan(0, durationInMins, 0));
            else
                options.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddMinutes(durationInMins));

            await _distributedCache.SetStringAsync(key, data, options);
        }

    }
}
