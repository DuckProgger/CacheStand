namespace Core.Metric;

public struct MetricsResult
{
    public TimeSpan TotalRequestTime { get; set; }
    
    public TimeSpan AverageRequestTime { get; set; }
    
    public TimeSpan AverageCacheTime { get; set; }
    
    public double CacheEfficiency { get; set; }
    
    public int RequestsPerSecond { get; set; }

    public double HitRate { get; set; }
    
    public int TotalRequests { get; set; }

    public int TotalReadRequests { get; set; }

    public int TotalCacheHits { get; set; }
    
    public TimeSpan AverageSerializationTime { get; set; }

    public TimeSpan AverageDeserializationTime { get; set; }

    public TimeSpan AverageReadCacheTime { get; set; }
    
    public TimeSpan AverageWriteCacheTime { get; set; }

    public TimeSpan AverageRepositoryReadTime { get; set; }

    public TimeSpan AverageRepositoryWriteTime { get; set; }
}