using Core.Metric;
using Core.Utils;
using Core.Wrappers;

namespace Core.Decorators;

public class CacheMetricsDecorator : ICacheWrapper
{
    private readonly ICacheWrapper cacheWrapper;
    private readonly MetricsRepository metricsRepository;

    public CacheMetricsDecorator(ICacheWrapper cacheWrapper, MetricsRepository metricsRepository)
    {
        this.cacheWrapper = cacheWrapper;
        this.metricsRepository = metricsRepository;
    }

    public async Task<TValue?> GetValueAsync<TValue>(string key)
    {
        using var profiler = new Profiler();
        var entry = await cacheWrapper.GetValueAsync<TValue>(key);
        metricsRepository.AddCacheCosts(profiler.ElapsedTime);
        metricsRepository.WriteCacheHit(entry is not null);
        return entry;
    }

    public async Task SetValueAsync<TValue>(string key, TValue value)
    {
        using var profiler = new Profiler();
        await cacheWrapper.SetValueAsync(key, value);
        metricsRepository.AddCacheCosts(profiler.ElapsedTime);
    }
}