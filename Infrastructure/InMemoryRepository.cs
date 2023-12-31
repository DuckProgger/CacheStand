﻿using Core.Data;

namespace Infrastructure;

public class InMemoryDataRepository : IDataRepository
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

    public Task<Entry> Update(Entry entry)
    {
        if(entries.ContainsKey(entry.Id))
        {
            entries[entry.Id] = entry;
        }

        return Task.FromResult(entry);
    }
}