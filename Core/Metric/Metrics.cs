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

    public TimeSpan CacheReadTime { get; set; }
    
    public TimeSpan CacheWriteTime { get; set; }

    public TimeSpan RepositoryReadTime { get; set; }

    public TimeSpan RepositoryWriteTime { get; set; }

    public void Clear()
    {
        RequestTime = default;
        CacheCosts = default;
        CacheHit = default;
        Timestamp = default;
        IsReadOperation = default;
        SerializationTime = default;
        DeserializationTime = default;
        CacheReadTime = default;
        CacheWriteTime = default;
        RepositoryReadTime = default;
        RepositoryWriteTime = default;
    }
}
