using Core.Metric;

namespace Console.Writers;

public class MetricsConsoleWriter : IMetricsWriter
{
    public void Write(MetricsResult metricsResult)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  TotalRequestTime:  {metricsResult.TotalRequestTime}
                                  AverageRequestTime:  {metricsResult.AverageRequestTime.TotalMicroseconds} мкс
                                  AverageCacheTime:    {metricsResult.AverageCacheTime.TotalMicroseconds} мкс
                                  CacheEfficiency:     {metricsResult.CacheEfficiency:.##} %
                                  Hit rate:            {metricsResult.HitRate:.##} %
                                  RPS:                 {metricsResult.RequestsPerSecond}
                                  Total requests:      {metricsResult.TotalRequests}
                                  Total read requests: {metricsResult.TotalReadRequests}
                                  Total hits:          {metricsResult.TotalCacheHits}
                                  """);
        System.Console.WriteLine();
        System.Console.WriteLine($"""
                                  Average GetDbTime:           {metricsResult.AverageGetDbTime.TotalMicroseconds} мкс
                                  Average SetDbTime:           {metricsResult.AverageSetDbTime.TotalMicroseconds} мкс
                                  Average GetCacheTime:        {metricsResult.AverageGetCacheTime.TotalMicroseconds} мкс
                                  Average SetCacheTime:        {metricsResult.AverageSetCacheTime.TotalMicroseconds} мкс
                                  Average SerializationTime:   {metricsResult.AverageSerializationTime.TotalMicroseconds} мкс
                                  Average DeserializationTime: {metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс
                                  """);
    }
}