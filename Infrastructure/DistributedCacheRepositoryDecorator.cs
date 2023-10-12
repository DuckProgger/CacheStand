using System.Diagnostics;
using Core.Data;
using Core.Metric;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure;
public class DistributedCacheRepositoryDecorator : IRepository
{
    private readonly IRepository repository;
    private readonly IDistributedCache cache;
    private readonly MetricsStorage metricsStorage;

    public DistributedCacheRepositoryDecorator(IRepository repository,
        IDistributedCache cache,
        MetricsStorage metricsStorage)
    {
        this.repository = repository;
        this.cache = cache;
        this.metricsStorage = metricsStorage;
    }

    public async Task<Entry?> Get(int id)
    {
        var now = DateTime.Now;
        var requestStopwatch = new Stopwatch();
        requestStopwatch.Start();

        var cacheStopwatch = new Stopwatch();
        cacheStopwatch.Start();

        var entryStrFromCache = await cache.GetStringAsync(id.ToString());

        if (entryStrFromCache is null)
        {
            cacheStopwatch.Stop();

            var entry = await repository.Get(id);

            cacheStopwatch.Start();
            var serializedEntry = JsonConvert.SerializeObject(entry);
            await cache.SetStringAsync(entry.Id.ToString(), serializedEntry);
            cacheStopwatch.Stop();
            
            requestStopwatch.Stop();
            metricsStorage.Add(new Metrics(requestStopwatch.ElapsedTicks, cacheStopwatch.ElapsedTicks, false, now));

            return entry;
        }

        var entryFromCache = JsonConvert.DeserializeObject<Entry>(entryStrFromCache)!;

        cacheStopwatch.Stop();
        requestStopwatch.Stop();
        metricsStorage.Add(new Metrics(requestStopwatch.ElapsedTicks, cacheStopwatch.ElapsedTicks, true, now));

        return entryFromCache;
    }

    public async Task<Entry> Create(Entry entry)
    {
        return await repository.Create(entry);
    }

    public async Task<Entry> Update(Entry entry)
    {
        return await repository.Update(entry);
    }
}
