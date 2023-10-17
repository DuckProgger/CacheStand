using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Core.Wrappers;

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache cache;
    private readonly DistributedCacheEntryOptions options;

    public DistributedCacheWrapper(IDistributedCache cache, DistributedCacheEntryOptions options)
    {
        this.cache = cache;
        this.options = options;
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
        await cache.SetStringAsync(key, serializedValue, options);
    }
}