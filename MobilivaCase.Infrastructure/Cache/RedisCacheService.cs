using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;


namespace MobilivaCase.Infrastructure.Cache
{
    public class RedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> Get<T>(string key)
        {
           var res =  await _cache.GetStringAsync(key);
           return string.IsNullOrEmpty(res)  ? default : JsonSerializer.Deserialize<T>(res);
        }

        public async Task Set(string key, object obj, DistributedCacheEntryOptions options)
        {
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(obj), options);
        }

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Refresh(string key)
        {
            throw new NotImplementedException();
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

       

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
