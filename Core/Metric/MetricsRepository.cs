using DateTime = System.DateTime;

namespace Core.Metric;

public class MetricsRepository
{
    private readonly MetricsStorage metricsStorage = new();
    private Metrics currentMetrics;
    
    public void StartNew()
    {
        currentMetrics = new()
        {
            Timestamp = DateTime.Now
        };
    }

    public void EndWrite()
    {
        currentMetrics.RequestTime = DateTime.Now - currentMetrics.Timestamp;
        metricsStorage.Add(currentMetrics);
    }
    
    public IEnumerable<Metrics> GetMetrics() => metricsStorage.GetAll();

    public void WriteCacheHit(bool cacheHit) => currentMetrics.CacheHit = cacheHit;

    public void WriteIsReadOperation(bool isReadOperation) => currentMetrics.IsReadOperation = isReadOperation;
    
    public void AddCacheCosts(TimeSpan cacheCosts) => currentMetrics.CacheCosts += cacheCosts;

    public void AddSerializationTime(TimeSpan serializationTime) => currentMetrics.SerializationTime += serializationTime;

    public void AddDeserializationTime(TimeSpan deserializationTime) => currentMetrics.DeserializationTime += deserializationTime;

    public void AddCacheReadTime(TimeSpan cacheReadTime) => currentMetrics.CacheReadTime += cacheReadTime;

    public void AddCacheWriteTime(TimeSpan cacheWriteTime) => currentMetrics.CacheWriteTime += cacheWriteTime;

    public void AddRepositoryReadTime(TimeSpan repositoryReadTime) => currentMetrics.RepositoryReadTime += repositoryReadTime;

    public void AddRepositoryWriteTime(TimeSpan repositoryWriteTime) => currentMetrics.RepositoryWriteTime += repositoryWriteTime;
}
