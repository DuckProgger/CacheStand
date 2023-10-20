using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Console;

/// <summary>
/// Фабрика создания контекста для Dependency Injection.
/// </summary>
public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public static ApplicationContext CreateDbContext()
    {
        return new ApplicationContext(
            new DbContextOptionsBuilder<ApplicationContext>()
                .UseSqlite(Settings.ConnectionStrings.Database)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                //.EnableSensitiveDataLogging()
                //.LogTo(System.Console.WriteLine, LogLevel.Information)
                .Options);
    }

    public ApplicationContext CreateDbContext(string[] args) => CreateDbContext();
}