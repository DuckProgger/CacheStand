using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Core.Services;

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache cache;
    private readonly DistributedCacheEntryOptions cacheOptions;

    public DistributedCacheWrapper(IDistributedCache cache)
    {
        this.cache = cache;
        cacheOptions = new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromSeconds(100)
        };
    }

    public async Task<TValue?> GetValueAsync<TValue>(string key)
    {
        var entryStr = await cache.GetStringAsync(key);
        return entryStr is not null
            ? JsonConvert.DeserializeObject<TValue>(entryStr)
            : default;
    }

    public async Task SetValueAsync<TValue>(string key, TValue value)
    {
        var serializedValue = JsonConvert.SerializeObject(value);
        await cache.SetStringAsync(key, serializedValue, cacheOptions);
    }
}