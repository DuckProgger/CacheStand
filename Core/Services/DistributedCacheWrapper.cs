using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Core.Services;

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache cache;

    public DistributedCacheWrapper(IDistributedCache cache)
    {
        this.cache = cache;
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
        await cache.SetStringAsync(key, serializedValue);
    }
}