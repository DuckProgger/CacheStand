using Core.Data;

namespace Infrastructure;

public class InMemoryRepository : IRepository
{
    private readonly Dictionary<int, Entry> entries = new();
    private static int idCount = 0;
    
    public Task<Entry?> Get(int id)
    {
        entries.TryGetValue(id, out var entry);
        return Task.FromResult(entry);
    }

    public Task<Entry> Create(Entry entry)
    {
        idCount++;
        entries.Add(idCount, entry);
        entry.Id = idCount;
        return Task.FromResult(entry);
    }

    public async Task<Entry> Update(Entry entry)
    {
        throw new NotImplementedException();
    }
}