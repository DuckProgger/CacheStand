namespace Core.Metric;

public class MetricsResult
{
    public TimeSpan TotalRequestTime { get; set; }
    
    public TimeSpan AverageRequestTime { get; set; }
    
    public TimeSpan AverageCacheTime { get; set; }
    
    public double CacheEfficiency { get; set; }
    
    public int RequestsPerSecond { get; set; }

    public double HitRate { get; set; }

    public double QueryAcceleration { get; set; }

    public int TotalRequests { get; set; }

    public int TotalReadRequests { get; set; }

    public int TotalCacheHits { get; set; }
    
    public TimeSpan AverageSerializationTime { get; set; }

    public TimeSpan AverageDeserializationTime { get; set; }

    public TimeSpan AverageGetCacheTime { get; set; }
    
    public TimeSpan AverageSetCacheTime { get; set; }

    public TimeSpan AverageGetDbTime { get; set; }

    public TimeSpan AverageSetDbTime { get; set; }
}