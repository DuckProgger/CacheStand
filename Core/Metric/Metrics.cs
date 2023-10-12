namespace Core.Metric;

public class Metrics
{
    public Metrics(TimeSpan requestTime, TimeSpan cacheCosts, bool cacheHit, DateTime timestamp)
    {
        RequestTime = requestTime;
        CacheCosts = cacheCosts;
        CacheHit = cacheHit;
        Timestamp = timestamp;
    }
    public Metrics(long requestTimeTicks, long cacheCostsTicks, bool cacheHit, DateTime timestamp)
    {
        RequestTime = new TimeSpan(requestTimeTicks);
        CacheCosts = new TimeSpan(cacheCostsTicks);
        CacheHit = cacheHit;
        Timestamp = timestamp;
    }
    
    public TimeSpan RequestTime { get; set; }

    public TimeSpan CacheCosts { get; set; }

    public bool CacheHit { get; set; }

    public DateTime Timestamp { get; set; }
}
