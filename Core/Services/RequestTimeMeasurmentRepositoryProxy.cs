using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.Services;

public class RequestTimeMeasurmentRepositoryProxy : IRepository
{
    private readonly IRepository repository;
    private readonly Metrics metrics;

    public RequestTimeMeasurmentRepositoryProxy(IRepository repository, Metrics metrics)
    {
        this.repository = repository;
        this.metrics = metrics;
    }

    public async Task<Entry?> Get(int id)
    {
        metrics.Timestamp = DateTime.Now;
        using var profiler = new Profiler();
        var entry = await repository.Get(id);
        metrics.RequestTime = profiler.ElapsedTime;
        metrics.IsReadOperation = true;
        return entry;
    }

    public async Task<Entry> Create(Entry entry)
    {
        metrics.Timestamp = DateTime.Now;
        using var profiler = new Profiler();
        var createdEntry = await repository.Create(entry);
        metrics.RequestTime = profiler.ElapsedTime;
        return createdEntry;
    }

    public async Task<Entry> Update(Entry entry)
    {
        metrics.Timestamp = DateTime.Now;
        using var profiler = new Profiler();
        var updatedEntry = await repository.Update(entry);
        metrics.RequestTime = profiler.ElapsedTime;
        return updatedEntry;
    }
}