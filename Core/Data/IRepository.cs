namespace Core.Data;

public interface IRepository
{
    public Task<Entry?> Get(int id);

    public Task<Entry> Create(Entry entry);
    
    public Task<Entry> Update(Entry entry);
}
