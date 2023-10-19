using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.Proxies;

public class RepositoryTimeMeasurmentProxy : IRepository
{
    private readonly IRepository repository;
    private readonly Metrics metrics;

    public RepositoryTimeMeasurmentProxy(IRepository repository, Metrics metrics)
    {
        this.repository = repository;
        this.metrics = metrics;
    }
    
    public async Task<Entry?> Get(int id)
    {
        var profiler = new Profiler();
        var entry = await repository.Get(id);
        metrics.GetDbTime = profiler.ElapsedTime;
        return entry;
    }

    public async Task<Entry> Create(Entry entry)
    {
        return await repository.Create(entry);
    }

    public async Task<Entry> Update(Entry entry)
    {
        using var profiler = new Profiler();
        var updatedEntry = await repository.Update(entry);
        metrics.SetDbTime = profiler.ElapsedTime;
        return updatedEntry;
        
    }
}