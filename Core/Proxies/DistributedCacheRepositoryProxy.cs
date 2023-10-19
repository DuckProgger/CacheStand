using System.Diagnostics;
using Core.Data;
using Core.Metric;
using Core.Utils;
using Core.Wrappers;

namespace Core.Proxies;

public class DistributedCacheRepositoryProxy : IRepository
{
    private readonly IRepository repository;
    private readonly IDistributedCacheWrapper cacheWrapper;
    private readonly Metrics metrics;

    public DistributedCacheRepositoryProxy(IRepository repository,
        IDistributedCacheWrapper cacheWrapper, Metrics metrics)
    {
        this.repository = repository;
        this.cacheWrapper = cacheWrapper;
        this.metrics = metrics;
    }

    public async Task<Entry?> Get(int id)
    {
        var entryFromCache = await cacheWrapper.GetValueAsync<Entry>(id.ToString());
        if (entryFromCache is not null) return entryFromCache;
        var sw = new Stopwatch();
        sw.Start();
        var entry = await repository.Get(id);
        metrics.GetDbTime = sw.Elapsed;
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), entry);
        return entry;
    }

    public async Task<Entry> Create(Entry entry)
    {
        var createdEntry = await repository.Create(entry);
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), createdEntry);
        return createdEntry;
    }

    public async Task<Entry> Update(Entry entry)
    {
        using var profiler = new Profiler();
        var updatedEntry = await repository.Update(entry);
        metrics.SetDbTime = profiler.ElapsedTime;
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), updatedEntry);
        return updatedEntry;
    }
}
