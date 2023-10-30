using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class DbDataRepository : IDataRepository
{
    private readonly ApplicationContext dbContext;
    private readonly DbSet<Entry> dbSet;
    public DbDataRepository(ApplicationContext dbContext)
    {
        this.dbContext = dbContext;
        dbSet = dbContext.Set<Entry>();
    }

    public virtual IQueryable<Entry> Items => dbSet;

    private async Task SaveChangesAsync(CancellationToken cancel = default)
    {
        await dbContext.SaveChangesAsync(cancel).ConfigureAwait(false);
    }

    public async Task<Entry?> Get(int id)
    {
        //await Task.Delay(1);
        dbContext.ChangeTracker.Clear();
        return await Items
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id)
            .ConfigureAwait(false);
    }

    public async Task<Entry> Create(Entry entry)
    {
        dbSet.Add(entry);
        await SaveChangesAsync();
        return entry;
    }

    public async Task<Entry> Update(Entry entry)
    {
        dbContext.ChangeTracker.Clear();
        dbSet.Update(entry);
        await SaveChangesAsync();
        return entry;
    }
}
