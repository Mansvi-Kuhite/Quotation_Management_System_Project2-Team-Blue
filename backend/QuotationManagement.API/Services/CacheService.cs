using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace QuotationManagement.API.Services
{
    public class CacheServices
    {
        private readonly IDistributedCache _cache;

        private readonly IDatabase _db;

        public CacheServices(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase(); // ✅ initialize _db
        }

        public CacheServices(IDistributedCache cache)
        {
            _cache = cache;
        }

        // Get cache, returns null if key not found
    

        // Set cache as string
        public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            string json = JsonSerializer.Serialize(value); // convert object to JSON string
            await _db.StringSetAsync(key, json, expiry);   // ⚡ store as string

        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);     // ⚡ read as string
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value);  // convert JSON back to object
        }



        // Remove cache
        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}