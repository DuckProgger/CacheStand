namespace Core.Metric;

public struct Metrics
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
}
