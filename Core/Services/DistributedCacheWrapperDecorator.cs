using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.Services;

public class DistributedCacheWrapperDecorator : IDistributedCacheWrapper
{
    private readonly IDistributedCacheWrapper cacheWrapper;
    private readonly Metrics metrics;

    public DistributedCacheWrapperDecorator(IDistributedCacheWrapper cacheWrapper, Metrics metrics)
    {
        this.cacheWrapper = cacheWrapper;
        this.metrics = metrics;
    }

    public async Task<TValue?> GetValueAsync<TValue>(string key)
    {
        using var profiler = new Profiler();
        var entry = await cacheWrapper.GetValueAsync<TValue>(key);
        metrics.CacheCosts += profiler.ElapsedTime;
        metrics.CacheHit = entry is not null;
        return entry;
    }

    public async Task SetValueAsync<TValue>(string key, TValue value)
    {
        using var profiler = new Profiler();
        await cacheWrapper.SetValueAsync(key, value);
        metrics.CacheCosts += profiler.ElapsedTime;
    }
}