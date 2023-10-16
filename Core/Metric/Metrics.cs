namespace Core.Metric;

public record Metrics
{
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
}
