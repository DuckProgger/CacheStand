namespace Core.Metric;

public record Metrics
{
    public TimeSpan RequestTime { get; set; }

    public TimeSpan CacheCosts { get; set; }

    public bool CacheHit { get; set; }

    public DateTime Timestamp { get; set; }
    
    public bool IsReadOperation { get; set; }
    
    public TimeSpan SerializationTime { get; set; }

    public TimeSpan DeserializationTime { get; set; }

    public TimeSpan GetCacheTime { get; set; }
    
    public TimeSpan SetCacheTime { get; set; }

    public TimeSpan GetDbTime { get; set; }

    public TimeSpan SetDbTime { get; set; }

    public void Clear()
    {
        RequestTime = default;
        CacheCosts = default;
        CacheHit = default;
        Timestamp = default;
        IsReadOperation = default;
        SerializationTime = default;
        DeserializationTime = default;
        GetCacheTime = default;
        SetCacheTime = default;
        GetDbTime = default;
        SetDbTime = default;
    }
}
