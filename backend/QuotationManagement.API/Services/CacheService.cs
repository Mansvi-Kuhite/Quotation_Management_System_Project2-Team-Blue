using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace QuotationManagement.API.Services
{
    public class CacheServices
    {
        private readonly IDistributedCache? _cache;
        private readonly IDatabase? _db;

        public CacheServices(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public CacheServices(IDistributedCache cache)
        {
            _cache = cache;
        }

        public CacheServices()
        {
            // Fallback mode when no cache provider is available.
        }

        // Get cache, returns null if key not found
        public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            if (_db == null) return;

            string json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            if (_db == null) return default;

            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value!);
        }

        // Remove cache
        public async Task RemoveCacheAsync(string key)
        {
            if (_cache != null)
            {
                await _cache.RemoveAsync(key);
                return;
            }

            if (_db != null)
            {
                await _db.KeyDeleteAsync(key);
            }
        }
    }
}
