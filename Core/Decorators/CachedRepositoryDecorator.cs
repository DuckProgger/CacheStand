using Core.Data;
using Core.Wrappers;

namespace Core.Decorators;

public class CachedDataRepositoryDecorator : IDataRepository
{
    private readonly IDataRepository dataRepository;
    private readonly ICacheWrapper cacheWrapper;

    public CachedDataRepositoryDecorator(IDataRepository dataRepository, ICacheWrapper cacheWrapper)
    {
        this.dataRepository = dataRepository;
        this.cacheWrapper = cacheWrapper;
    }

    public async Task<Entry?> Get(int id)
    {
        var entryFromCache = await cacheWrapper.GetValueAsync<Entry>(id.ToString());
        if (entryFromCache is not null) return entryFromCache;
        var entry = await dataRepository.Get(id);
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), entry);
        return entry;
    }

    public async Task<Entry> Create(Entry entry)
    {
        var createdEntry = await dataRepository.Create(entry);
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), createdEntry);
        return createdEntry;
    }

    public async Task<Entry> Update(Entry entry)
    {
        var updatedEntry = await dataRepository.Update(entry);
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), updatedEntry);
        return updatedEntry;
    }
}
