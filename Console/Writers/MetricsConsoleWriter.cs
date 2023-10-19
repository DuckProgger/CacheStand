using Core.Metric;

namespace Console.Writers;

public class MetricsConsoleWriter : IMetricsWriter
{
    public void Write(MetricsResult metricsResult)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Total request time:...{metricsResult.TotalRequestTime}
                                  Average request time:.{metricsResult.AverageRequestTime.TotalMicroseconds} мкс
                                  Average cache costs:..{metricsResult.AverageCacheTime.TotalMicroseconds} мкс
                                  Cache efficiency:.....{metricsResult.CacheEfficiency:.##} %
                                  Hit rate:.............{metricsResult.HitRate:.##} %
                                  RPS:..................{metricsResult.RequestsPerSecond}
                                  Total requests:.......{metricsResult.TotalRequests}
                                  Total read requests:..{metricsResult.TotalReadRequests}
                                  Total hits:...........{metricsResult.TotalCacheHits}
                                  """);
        System.Console.WriteLine();
        System.Console.WriteLine($"""
                                  Average repository read time:..{metricsResult.AverageRepositoryReadTime.TotalMicroseconds} мкс
                                  Average cache read time:.......{metricsResult.AverageReadCacheTime.TotalMicroseconds} мкс
                                  Average deserialization time:..{metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс
                                  Average repository write time:.{metricsResult.AverageRepositoryWriteTime.TotalMicroseconds} мкс
                                  Average cache write time:......{metricsResult.AverageWriteCacheTime.TotalMicroseconds} мкс
                                  Average serialization time:....{metricsResult.AverageSerializationTime.TotalMicroseconds} мкс
                                  """);
    }
}