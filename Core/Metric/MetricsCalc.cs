namespace Core.Metric;

public class MetricsCalc
{
    private readonly List<Metrics> metricList;

    public MetricsCalc(IEnumerable<Metrics> metrics)
    {
        metricList = new List<Metrics>(metrics);
    }

    public double GetQueryAcceleration()
    {
        var averageCacheMissQueryTime = GetAverageCacheMissQueryTime();
        var averageCacheHitQueryTime = GetAverageCacheHitQueryTime();
        return (averageCacheMissQueryTime - averageCacheHitQueryTime) / averageCacheMissQueryTime * 100.0;
    }

    public double GetHitRate()
    {
        var queriesCount = metricList.Count;
        var cacheMissCount = metricList.Count(x => !x.CacheHit);
        return (double)(queriesCount - cacheMissCount) / queriesCount * 100.0;
    }

    public int GetRps()
    {
        if (!metricList.Any()) return 0;
        return (int)metricList
            .GroupBy(x => x.Timestamp.Second)
            .Select(x => x.Count())
            .Average();
    }

    public int GetTotalRequests()
    {
        return metricList.Count;
    }

    public int GetTotalCacheHits()
    {
        return metricList.Any(x => x.CacheHit) ? metricList.Count(x => x.CacheHit) : 0;
    }

    private TimeSpan GetAverageCacheMissQueryTime()
    {
        if (metricList.All(x => x.CacheHit)) return TimeSpan.Zero;
        var cacheMissMetricsTicks = metricList
            .Where(x => !x.CacheHit)
            .Select(x => x.RequestTime.Ticks - x.CacheCosts.Ticks)
            .Average();
        return new TimeSpan((int)cacheMissMetricsTicks);
    }

    private TimeSpan GetAverageCacheHitQueryTime()
    {
        if (metricList.All(x => !x.CacheHit)) return TimeSpan.Zero;
        var cacheHitMetricsTicks = metricList
            .Where(x => x.CacheHit)
            .Select(x => x.RequestTime.Ticks)
            .Average();
        return new TimeSpan((int)cacheHitMetricsTicks);
    }
}