namespace Core.Metric;

public class MetricsResult
{
    public int RequestsPerSecond { get; set; }

    public double HitRate { get; set; }

    public double QueryAcceleration { get; set; }

    public int TotalRequests { get; set; }

    public int TotalReadRequests { get; set; }

    public int TotalCacheHits { get; set; }
}