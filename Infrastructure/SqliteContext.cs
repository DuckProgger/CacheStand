using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class SqliteContext : ApplicationContext
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SqliteContext(DbContextOptions<ApplicationContext> options) : base(options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }
}   

