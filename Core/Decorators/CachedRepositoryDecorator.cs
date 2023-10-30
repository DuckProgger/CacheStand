using Core.Data;
using Core.Wrappers;

namespace Core.Decorators;

public class CachedRepositoryDecorator : IRepository
{
    private readonly IRepository repository;
    private readonly ICacheWrapper cacheWrapper;

    public CachedRepositoryDecorator(IRepository repository, ICacheWrapper cacheWrapper)
    {
        this.repository = repository;
        this.cacheWrapper = cacheWrapper;
    }

    public async Task<Entry?> Get(int id)
    {
        var entryFromCache = await cacheWrapper.GetValueAsync<Entry>(id.ToString());
        if (entryFromCache is not null) return entryFromCache;
        var entry = await repository.Get(id);
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
        var updatedEntry = await repository.Update(entry);
        await cacheWrapper.SetValueAsync(entry.Id.ToString(), updatedEntry);
        return updatedEntry;
    }
}
