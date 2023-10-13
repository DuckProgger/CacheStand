namespace Core.Metric;

public record Metrics
{
    public Metrics() { }

    public Metrics(long requestTimeTicks, long cacheCostsTicks, bool cacheHit, DateTime timestamp)
        : this(new TimeSpan(requestTimeTicks), new TimeSpan(cacheCostsTicks), cacheHit, timestamp) { }

    public Metrics(TimeSpan RequestTime, TimeSpan CacheCosts, bool CacheHit, DateTime Timestamp)
    {
        this.RequestTime = RequestTime;
        this.CacheCosts = CacheCosts;
        this.CacheHit = CacheHit;
        this.Timestamp = Timestamp;
    }

    public TimeSpan RequestTime { get; set; }

    public TimeSpan CacheCosts { get; set; }

    public bool CacheHit { get; set; }

    public DateTime Timestamp { get; set; }

    public void Clear()
    {
        RequestTime = default;
        CacheCosts = default;
        CacheHit = default;
        Timestamp = default;
    }

    //public object Clone()
    //{
    //    throw new NotImplementedException();
    //}
    public void Deconstruct(out TimeSpan RequestTime, out TimeSpan CacheCosts, out bool CacheHit, out DateTime Timestamp)
    {
        RequestTime = this.RequestTime;
        CacheCosts = this.CacheCosts;
        CacheHit = this.CacheHit;
        Timestamp = this.Timestamp;
    }
}
