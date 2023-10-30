using Core.Metric;
using Core.Utils;
using Core.Wrappers;

namespace Core.Decorators;

public class CacheMetricsDecorator : ICacheWrapper
{
    private readonly ICacheWrapper cacheWrapper;
    private readonly MetricsWriter metricsWriter;

    public CacheMetricsDecorator(ICacheWrapper cacheWrapper, MetricsWriter metricsWriter)
    {
        this.cacheWrapper = cacheWrapper;
        this.metricsWriter = metricsWriter;
    }

    public async Task<TValue?> GetValueAsync<TValue>(string key)
    {
        using var profiler = new Profiler();
        var entry = await cacheWrapper.GetValueAsync<TValue>(key);
        metricsWriter.AddCacheCosts(profiler.ElapsedTime);
        metricsWriter.WriteCacheHit(entry is not null);
        return entry;
    }

    public async Task SetValueAsync<TValue>(string key, TValue value)
    {
        using var profiler = new Profiler();
        await cacheWrapper.SetValueAsync(key, value);
        metricsWriter.AddCacheCosts(profiler.ElapsedTime);
    }
}