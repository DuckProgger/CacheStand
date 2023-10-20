using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;

namespace Console;

public class PostgresContextFactory : IDesignTimeDbContextFactory<PostgresContext>
{
    public static PostgresContext CreateDbContext()
    {
        var builder = new DbContextOptionsBuilder<ApplicationContext>();
        //builder.EnableSensitiveDataLogging().LogTo(System.Console.WriteLine, LogLevel.Information);
        builder.UseNpgsql(Settings.ConnectionStrings.PostgreSql);
        return new PostgresContext(builder.Options);
    }

    public PostgresContext CreateDbContext(string[] args) => CreateDbContext();
}

public class SqliteContextFactory : IDesignTimeDbContextFactory<SqliteContext>
{
    public static SqliteContext CreateDbContext()
    {
        var builder = new DbContextOptionsBuilder<ApplicationContext>();
        //builder.EnableSensitiveDataLogging().LogTo(System.Console.WriteLine, LogLevel.Information);
        builder.UseSqlite(Settings.ConnectionStrings.Sqlite);
        return new SqliteContext(builder.Options);
    }

    public SqliteContext CreateDbContext(string[] args) => CreateDbContext();
}