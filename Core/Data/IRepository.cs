namespace Core.Data;

public interface IDataRepository
{
    public Task<Entry?> Get(int id);

    public Task<Entry> Create(Entry entry);
    
    public Task<Entry> Update(Entry entry);
}
