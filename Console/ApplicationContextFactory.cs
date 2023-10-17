using System.Diagnostics;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
                .UseSqlite(GetConnectionString())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                //.EnableSensitiveDataLogging()
                //.LogTo(System.Console.WriteLine, LogLevel.Information)
                .Options);
    }


    internal static string GetConnectionString()
    {
        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.SetBasePath(AppContext.BaseDirectory);
        builder.AddJsonFile("appsettings.json");
        IConfigurationRoot config = builder.Build();
        return config.GetConnectionString("DefaultConnection");
    }

    /// <summary>
    /// Создание экземпляра ApplicationContext.
    /// </summary>
    public ApplicationContext CreateDbContext(string[] args) => CreateDbContext();
}