using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.Decorators;

public class RepositoryMetricsDecorator : IRepository
{
    private readonly IRepository repository;
    private readonly MetricsWriter metricsWriter;

    public RepositoryMetricsDecorator(IRepository repository, MetricsWriter metricsWriter)
    {
        this.repository = repository;
        this.metricsWriter = metricsWriter;
    }
    
    public async Task<Entry?> Get(int id)
    {
        var profiler = new Profiler();
        var entry = await repository.Get(id);
        metricsWriter.AddRepositoryReadTime(profiler.ElapsedTime);
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
        metricsWriter.AddRepositoryWriteTime(profiler.ElapsedTime);
        return updatedEntry;
    }
}