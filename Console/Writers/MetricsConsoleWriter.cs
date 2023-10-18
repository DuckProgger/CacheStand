using Core.Metric;

namespace Console.Writers;

public class MetricsConsoleWriter : IMetricsWriter
{
    public void Write(MetricsResult metricsResult)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Acc: {metricsResult.QueryAcceleration:.##} %
                                  HR: {metricsResult.HitRate:.##} %
                                  RPS: {metricsResult.RequestsPerSecond}
                                  Total requests: {metricsResult.TotalRequests}
                                  Total read requests: {metricsResult.TotalReadRequests}
                                  Total hits: {metricsResult.TotalCacheHits}
                                  """);
    }
}