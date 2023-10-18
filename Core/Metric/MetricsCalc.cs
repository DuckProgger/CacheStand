using Core.ExecutionStrategy;

namespace Core.Metric;

public class MetricsCalc
{
    private readonly List<Metrics> metricList;

    public MetricsCalc(IEnumerable<Metrics> metrics)
    {
        metricList = new List<Metrics>(metrics);
    }

    public MetricsResult Calculate(ExecutionStrategyType strategyType)
    {
        var queryAcceleration = GetQueryAcceleration();
        var hitRate = GetHitRate();
        var rps = strategyType == ExecutionStrategyType.RealTime
            ? GetLastRps()
            : GetAverageRps();
        var totalRequests = GetTotalRequests();
        var totalReadRequests = GetTotalReadRequests();
        var totalCacheHits = GetTotalCacheHits();

        return new MetricsResult()
        {
            QueryAcceleration = queryAcceleration,
            HitRate = hitRate,
            RequestsPerSecond = rps,
            TotalRequests = totalRequests,
            TotalReadRequests = totalReadRequests,
            TotalCacheHits = totalCacheHits,
        };
    }

    private double GetQueryAcceleration()
    {
        var averageCacheMissQueryTime = GetAverageCacheMissQueryTime();
        var averageCacheHitQueryTime = GetAverageCacheHitQueryTime();
        if (averageCacheMissQueryTime == TimeSpan.Zero || averageCacheHitQueryTime == TimeSpan.Zero)
            return 0;
        return (averageCacheMissQueryTime - averageCacheHitQueryTime) / averageCacheMissQueryTime * 100.0;
    }

    private double GetHitRate()
    {
        var readOperations = metricList
            .Where(x => x.IsReadOperation)
            .ToArray();
        var queriesCount = readOperations.Length;
        var cacheMissCount = readOperations.Count(x => !x.CacheHit);
        return (double)(queriesCount - cacheMissCount) / queriesCount * 100.0;
    }

    private int GetAverageRps()
    {
        if (!metricList.Any()) return 0;
        return (int)metricList
            .GroupBy(x => new { x.Timestamp.Day, x.Timestamp.Hour, x.Timestamp.Minute, x.Timestamp.Second })
            .Select(x => x.Count())
            .Average();
    }

    private int GetLastRps()
    {
        if (!metricList.Any()) return 0;
        var lastTimestamp = metricList.MaxBy(x => x.Timestamp)!.Timestamp;
        var lastSecondTimestamp = lastTimestamp.AddSeconds(-1);
        return metricList.Count(x => x.Timestamp > lastSecondTimestamp);
    }

    private int GetTotalRequests()
    {
        return metricList.Count;
    }

    private int GetTotalReadRequests()
    {
        return metricList.Count(x => x.IsReadOperation);
    }

    private int GetTotalCacheHits()
    {
        return metricList.Any(x => x.CacheHit) ? metricList.Count(x => x.CacheHit) : 0;
    }

    private TimeSpan GetAverageCacheMissQueryTime()
    {
        if (metricList.All(x => x.CacheHit || !x.IsReadOperation)) return TimeSpan.Zero;
        var cacheMissMetricsTicks = metricList
            .Where(x => !x.CacheHit && x.IsReadOperation)
            .Select(x => x.RequestTime.Ticks - x.CacheCosts.Ticks)
            .Average();
        return new TimeSpan((int)cacheMissMetricsTicks);
    }

    private TimeSpan GetAverageCacheHitQueryTime()
    {
        if (metricList.All(x => !x.CacheHit || !x.IsReadOperation)) return TimeSpan.Zero;
        var cacheHitMetricsTicks = metricList
            .Where(x => x.CacheHit && x.IsReadOperation)
            .Select(x => x.RequestTime.Ticks)
            .Average();
        return new TimeSpan((int)cacheHitMetricsTicks);
    }
}