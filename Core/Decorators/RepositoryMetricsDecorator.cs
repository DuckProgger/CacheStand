using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.Decorators;

public class DataRepositoryMetricsDecorator : IDataRepository
{
    private readonly IDataRepository dataRepository;
    private readonly MetricsRepository metricsRepository;

    public DataRepositoryMetricsDecorator(IDataRepository dataRepository, MetricsRepository metricsRepository)
    {
        this.dataRepository = dataRepository;
        this.metricsRepository = metricsRepository;
    }
    
    public async Task<Entry?> Get(int id)
    {
        var profiler = new Profiler();
        var entry = await dataRepository.Get(id);
        metricsRepository.AddRepositoryReadTime(profiler.ElapsedTime);
        return entry;
    }

    public async Task<Entry> Create(Entry entry)
    {
        return await dataRepository.Create(entry);
    }

    public async Task<Entry> Update(Entry entry)
    {
        using var profiler = new Profiler();
        var updatedEntry = await dataRepository.Update(entry);
        metricsRepository.AddRepositoryWriteTime(profiler.ElapsedTime);
        return updatedEntry;
    }
}