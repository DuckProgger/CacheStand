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
        var averageRequestTime = GetAverageRequestTime();
        var totalRequestTime = GetTotalRequestTime();
        var averageCacheCosts = GetAverageCacheCosts();
        var cacheEfficiency = GetCacheEfficiency();
        var hitRate = GetHitRate();
        var rps = strategyType == ExecutionStrategyType.RealTime
            ? GetLastRps()
            : GetAverageRps();
        var totalRequests = GetTotalRequests();
        var totalReadRequests = GetTotalReadRequests();
        var totalCacheHits = GetTotalCacheHits();
        var averageCacheWriteTime = GetAverageCacheWriteTime();
        var averageCacheReadTime = GetAverageCacheReadTime();
        var averageRepositoryWriteTime = GetAverageRepositoryWriteTime();
        var averageRepositoryReadTime = GetAverageRepositoryReadTime();
        var serializationTime = GetAverageSerializationTime();
        var deserializationTime = GetAverageDeserializationTime();

        return new MetricsResult()
        {
            AverageRequestTime = averageRequestTime,
            TotalRequestTime = totalRequestTime,
            AverageCacheTime = averageCacheCosts,
            CacheEfficiency = cacheEfficiency,
            HitRate = hitRate,
            RequestsPerSecond = rps,
            TotalRequests = totalRequests,
            TotalReadRequests = totalReadRequests,
            TotalCacheHits = totalCacheHits,
            AverageWriteCacheTime = averageCacheWriteTime,
            AverageReadCacheTime = averageCacheReadTime,
            AverageRepositoryWriteTime = averageRepositoryWriteTime,
            AverageRepositoryReadTime = averageRepositoryReadTime,
            AverageSerializationTime = serializationTime,
            AverageDeserializationTime = deserializationTime,
        };
    }

    private TimeSpan GetAverageRequestTime()
    {
        if (Empty()) return TimeSpan.Zero;
        var averagerRequetTimeTicks = metricList
            .Average(x => x.RequestTime.Ticks);
        return new TimeSpan((long)averagerRequetTimeTicks);
    }

    private TimeSpan GetTotalRequestTime()
    {
        if (Empty()) return TimeSpan.Zero;
        var averagerRequetTimeTicks = metricList
            .Sum(x => x.RequestTime.Ticks);
        return new TimeSpan((long)averagerRequetTimeTicks);
    }

    private TimeSpan GetAverageCacheCosts()
    {
        if (Empty()) return TimeSpan.Zero;
        var averagerRequetTimeTicks = metricList
            .Average(x => x.CacheCosts.Ticks);
        return new TimeSpan((long)averagerRequetTimeTicks);
    }

    private double GetCacheEfficiency()
    {
        if (metricList.All(x => x.CacheCosts == TimeSpan.Zero)) return 0;

        var averageRepositoryReadTime = GetAverageRepositoryReadTime();
        var estimatedRepositoryReadTimeWithoutCache = averageRepositoryReadTime * GetTotalReadRequests();

        var averageRepositoryWriteTime = GetAverageRepositoryWriteTime();
        var estimatedRepositoryWriteTimeWithoutCache = averageRepositoryWriteTime * GetTotalWriteRequests();

        var estimatedRepositoryTimeWithoutCache =
            estimatedRepositoryReadTimeWithoutCache + estimatedRepositoryWriteTimeWithoutCache;

        var requestTimeSumTicks = metricList.Sum(x => x.RequestTime.Ticks);
        var requestTimeSum = new TimeSpan(requestTimeSumTicks);
        return (estimatedRepositoryTimeWithoutCache - requestTimeSum) / estimatedRepositoryTimeWithoutCache * 100.0;
    }

    private double GetHitRate()
    {
        if (!ContainsReadOperation()) return 0;
        var readOperations = metricList
            .Where(x => x.IsReadOperation)
            .ToArray();
        var queriesCount = readOperations.Length;
        var cacheMissCount = readOperations.Count(x => !x.CacheHit);
        return (double)(queriesCount - cacheMissCount) / queriesCount * 100.0;
    }

    private int GetAverageRps()
    {
        if (Empty()) return 0;
        return (int)metricList
            .GroupBy(x => new { x.Timestamp.Day, x.Timestamp.Hour, x.Timestamp.Minute, x.Timestamp.Second })
            .Select(x => x.Count())
            .Average();
    }

    private int GetLastRps()
    {
        if (Empty()) return 0;
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
        return Empty() ? 0 : metricList.Count(x => x.IsReadOperation);
    }

    private int GetTotalWriteRequests()
    {
        return Empty() ? 0 : metricList.Count(x => !x.IsReadOperation);
    }

    private int GetTotalCacheHits()
    {
        return ContainsCacheHit() ? metricList.Count(x => x.CacheHit) : 0;
    }

    private TimeSpan GetAverageSerializationTime()
    {
        return Empty() ? TimeSpan.Zero : new TimeSpan((int)metricList.Average(x => x.SerializationTime.Ticks));
    }

    private TimeSpan GetAverageDeserializationTime()
    {
        return Empty() ? TimeSpan.Zero : new TimeSpan((int)metricList.Average(x => x.DeserializationTime.Ticks));
    }

    private TimeSpan GetAverageCacheReadTime()
    {
        return Empty() ? TimeSpan.Zero : new TimeSpan((int)metricList.Average(x => x.CacheReadTime.Ticks));
    }

    private TimeSpan GetAverageCacheWriteTime()
    {
        return Empty() ? TimeSpan.Zero : new TimeSpan((int)metricList.Average(x => x.CacheWriteTime.Ticks));
    }

    private TimeSpan GetAverageRepositoryReadTime()
    {
        return Empty() || metricList.All(x => x.CacheHit) || !ContainsReadOperation()
            ? TimeSpan.Zero
            : new TimeSpan((int)metricList
                .Where(x => !x.CacheHit && x.IsReadOperation)
                .Average(x => x.RepositoryReadTime.Ticks));
    }

    private TimeSpan GetAverageRepositoryWriteTime()
    {
        return Empty() || !ContainsWriteOperation()
            ? TimeSpan.Zero
            : new TimeSpan((int)metricList
                .Where(x => !x.IsReadOperation)
                .Average(x => x.RepositoryWriteTime.Ticks));
    }

    private bool Empty() => !metricList.Any();

    private bool ContainsReadOperation() => metricList.Any(x => x.IsReadOperation);

    private bool ContainsWriteOperation() => metricList.Any(x => !x.IsReadOperation);

    private bool ContainsCacheHit() => metricList.Any(x => x.CacheHit);
}