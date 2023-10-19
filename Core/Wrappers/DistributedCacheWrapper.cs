using System.Diagnostics;
using Core.Metric;
using Core.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Core.Wrappers;

public class DistributedCacheWrapper : IDistributedCacheWrapper
{
    private readonly IDistributedCache cache;
    private readonly DistributedCacheEntryOptions options;
    private readonly Metrics metrics;

    public DistributedCacheWrapper(IDistributedCache cache, DistributedCacheEntryOptions options, Metrics metrics)
    {
        this.cache = cache;
        this.options = options;
        this.metrics = metrics;
    }

    public async Task<TValue?> GetValueAsync<TValue>(string key)
    {
        using var profiler = new Profiler();
        var entryStr = await cache.GetStringAsync(key);
        metrics.GetCacheTime = profiler.ElapsedTime;
        if (entryStr is null) return default;
        profiler.Restart();
        var value = JsonConvert.DeserializeObject<TValue>(entryStr);
        metrics.DeserializationTime = profiler.ElapsedTime;
        return value;
    }

    public async Task SetValueAsync<TValue>(string key, TValue value)
    {
        using var profiler = new Profiler();
        var serializedValue = JsonConvert.SerializeObject(value);
        metrics.SerializationTime = profiler.ElapsedTime;
        profiler.Restart();
        await cache.SetStringAsync(key, serializedValue, options);
        metrics.SetCacheTime = profiler.ElapsedTime;
    }
}