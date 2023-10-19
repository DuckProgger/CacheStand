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
        var averageCacheTime = GetAverageCacheTime();
        var cacheEfficiency = GetCacheEfficiency();
        var queryAcceleration = GetQueryAcceleration();
        var hitRate = GetHitRate();
        var rps = strategyType == ExecutionStrategyType.RealTime
            ? GetLastRps()
            : GetAverageRps();
        var totalRequests = GetTotalRequests();
        var totalReadRequests = GetTotalReadRequests();
        var totalCacheHits = GetTotalCacheHits();
        var setCacheTime = AverageSetCacheTime();
        var getCacheTime = AverageGetCacheTime();
        var setDbTime = AverageSetDbTime();
        var getDbTime = AverageGetDbTime();
        var serializationTime = AverageSerializationTime();
        var deserializationTime = AverageDeserializationTime();

        return new MetricsResult()
        {
            AverageRequestTime = averageRequestTime,
            TotalRequestTime = totalRequestTime,
            AverageCacheTime = averageCacheTime,
            CacheEfficiency = cacheEfficiency,
            QueryAcceleration = queryAcceleration,
            HitRate = hitRate,
            RequestsPerSecond = rps,
            TotalRequests = totalRequests,
            TotalReadRequests = totalReadRequests,
            TotalCacheHits = totalCacheHits,
            AverageSetCacheTime = setCacheTime,
            AverageGetCacheTime = getCacheTime,
            AverageSetDbTime = setDbTime,
            AverageGetDbTime = getDbTime,
            AverageSerializationTime = serializationTime,
            AverageDeserializationTime = deserializationTime,
        };
    }
    
    private TimeSpan GetAverageRequestTime()
    {
        var averagerRequetTimeTicks = metricList
            .Where(x => x.IsReadOperation)
            .Average(x => x.RequestTime.Ticks);
        return new TimeSpan((long)averagerRequetTimeTicks);
    }
    
    private TimeSpan GetTotalRequestTime()
    {
        var averagerRequetTimeTicks = metricList
            .Where(x => x.IsReadOperation)
            .Sum(x => x.RequestTime.Ticks);
        return new TimeSpan((long)averagerRequetTimeTicks);
    }
    
    private TimeSpan GetAverageCacheTime()
    {
        var averagerRequetTimeTicks = metricList
            .Where(x => x.IsReadOperation)
            .Average(x => x.CacheCosts.Ticks);
        return new TimeSpan((long)averagerRequetTimeTicks);
    }
    
    private double GetCacheEfficiency()
    {
        //var averageRequestTime = GetAverageRequestTime();
        //var averageCacheTime = GetAverageCacheTime();
        //return (averageRequestTime - averageCacheTime) / averageRequestTime * 100.0;
        
        var averageDbTime = AverageGetDbTime();
        var estimatedTimeWithoutCache = averageDbTime * metricList.Count;
        var requestTimeSumTicks = metricList.Sum(x => x.RequestTime.Ticks);
        var requestTimeSum = new TimeSpan(requestTimeSumTicks);
        return (estimatedTimeWithoutCache - requestTimeSum) / estimatedTimeWithoutCache * 100.0;
    }

    //private double GetQueryAcceleration()
    //{
    //    var averageQueryTimeWithoutCache = GetAverageQueryTimeWithoutCache();
    //    var averageQueryTimeWithCache = GetAverageQueryTimeWithCache();
    //    if (averageQueryTimeWithoutCache == TimeSpan.Zero || averageQueryTimeWithCache == TimeSpan.Zero)
    //        return 0;
    //    return (averageQueryTimeWithoutCache - averageQueryTimeWithCache) / averageQueryTimeWithoutCache * 100.0;
    //}
    
    private double GetQueryAcceleration()
    {
        var totalQueryTimeWithoutCache = GetTotalQueryTimeWithoutCache();
        var totalQueryTimeWithCache = GetTotalQueryTimeWithCache();
        if (totalQueryTimeWithoutCache == TimeSpan.Zero || totalQueryTimeWithCache == TimeSpan.Zero)
            return 0;
        return (totalQueryTimeWithoutCache - totalQueryTimeWithCache) / totalQueryTimeWithoutCache * 100.0;
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

    private TimeSpan GetAverageQueryTimeWithoutCache()
    {
        //if (metricList.All(x => x.CacheHit || !x.IsReadOperation)) return TimeSpan.Zero;
        //var cacheMissMetricsTicks = metricList
        //    .Where(x => !x.CacheHit && x.IsReadOperation)
        //    .Select(x => x.RequestTime.Ticks - x.CacheCosts.Ticks)
        //    .Average();
        //return new TimeSpan((int)cacheMissMetricsTicks);

        var averageQueryTime = metricList
            .Where(x => !x.CacheHit)
            .Select(x => x.SetDbTime.Ticks + x.GetDbTime.Ticks)
            .Average();
        return new TimeSpan((int)averageQueryTime);
    }
    
    private TimeSpan GetAverageQueryTimeWithCache()
    {
        //if (metricList.All(x => !x.CacheHit || !x.IsReadOperation)) return TimeSpan.Zero;
        //var cacheHitMetricsTicks = metricList
        //    .Where(x => x.CacheHit && x.IsReadOperation)
        //    .Select(x => x.RequestTime.Ticks)
        //    .Average();
        //return new TimeSpan((int)cacheHitMetricsTicks);
        var averageQueryTime = metricList
            //.Where(x => x.CacheHit)
            .Select(x => x.GetCacheTime.Ticks + x.SetCacheTime.Ticks + x.SerializationTime.Ticks + x.DeserializationTime.Ticks)
            .Average();
        return new TimeSpan((int)averageQueryTime);
    }
    
    private TimeSpan GetTotalQueryTimeWithoutCache()
    {
        var averageQueryTime = metricList
            .Where(x => !x.CacheHit)
            .Select(x => x.SetDbTime.Ticks + x.GetDbTime.Ticks)
            .Sum();
        return new TimeSpan((int)averageQueryTime);
    }
    
    private TimeSpan GetTotalQueryTimeWithCache()
    {
        var averageQueryTime = metricList
            .Select(x => x.GetCacheTime.Ticks + x.SetCacheTime.Ticks + x.SerializationTime.Ticks + x.DeserializationTime.Ticks)
            .Sum();
        return new TimeSpan((int)averageQueryTime);
    }

    private TimeSpan AverageSerializationTime() => new((int)metricList.Average(x => x.SerializationTime.Ticks));

    private TimeSpan AverageDeserializationTime() => new((int)metricList.Average(x => x.DeserializationTime.Ticks));

    private TimeSpan AverageGetCacheTime() => new((int)metricList.Average(x => x.GetCacheTime.Ticks));

    private TimeSpan AverageSetCacheTime() => new((int)metricList.Average(x => x.SetCacheTime.Ticks));

    private TimeSpan AverageGetDbTime() => new((int)metricList.Where(x => !x.CacheHit).Average(x => x.GetDbTime.Ticks));

    private TimeSpan AverageSetDbTime() => new((int)metricList.Where(x => !x.CacheHit).Average(x => x.SetDbTime.Ticks));
}