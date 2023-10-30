using DateTime = System.DateTime;

namespace Core.Metric;

public class MetricsWriter
{
    private Metrics metrics;

    public void Begin()
    {
        metrics = new()
        {
            Timestamp = DateTime.Now
        };
    }

    public Metrics EndAndGet()
    {
        metrics.RequestTime = DateTime.Now - metrics.Timestamp;
        return metrics;
    }

    public void WriteCacheHit(bool cacheHit) => metrics.CacheHit = cacheHit;

    public void WriteIsReadOperation(bool isReadOperation) => metrics.IsReadOperation = isReadOperation;
    
    public void AddCacheCosts(TimeSpan cacheCosts) => metrics.CacheCosts += cacheCosts;

    public void AddSerializationTime(TimeSpan serializationTime) => metrics.SerializationTime += serializationTime;

    public void AddDeserializationTime(TimeSpan deserializationTime) => metrics.DeserializationTime += deserializationTime;

    public void AddCacheReadTime(TimeSpan cacheReadTime) => metrics.CacheReadTime += cacheReadTime;

    public void AddCacheWriteTime(TimeSpan cacheWriteTime) => metrics.CacheWriteTime += cacheWriteTime;

    public void AddRepositoryReadTime(TimeSpan repositoryReadTime) => metrics.RepositoryReadTime += repositoryReadTime;

    public void AddRepositoryWriteTime(TimeSpan repositoryWriteTime) => metrics.RepositoryWriteTime += repositoryWriteTime;
}
