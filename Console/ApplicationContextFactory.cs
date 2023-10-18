using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Console;

/// <summary>
/// Фабрика создания контекста для Dependency Injection.
/// </summary>
public class ApplicationContextFactory
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
}