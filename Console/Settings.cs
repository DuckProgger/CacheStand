using Core.ExecutionStrategy;
using Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Console;

public static class Settings
{
    private static readonly IConfigurationRoot config;

    static Settings()
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(AppContext.BaseDirectory);
        builder.AddJsonFile("appsettings.json");
        config = builder.Build();
    }

    public static class Seeding
    {
        public static int DataCount => int.Parse(config["Seeding:DataCount"]!);

        public static int MinStringLength => int.Parse(config["Seeding:MinStringLength"]!);

        public static int MaxStringLength => int.Parse(config["Seeding:MaxStringLength"]!);

        public static int MinBytesLength => int.Parse(config["Seeding:MinBytesLength"]!);

        public static int MaxBytesLength => int.Parse(config["Seeding:MaxBytesLength"]!);
    }

    public static class ExecutionOptions
    {
        public static ExecutionStrategyType ExecutionType =>
            Enum.Parse<ExecutionStrategyType>(config["ExecutionOptions:ExecutionType"]!);

        public static int UpdateOperationProbable => int.Parse(config["ExecutionOptions:UpdateOperationProbable"]!);

        public static class RealTime
        {
            public static TimeSpan RequestCycleTimeMs =>
                TimeSpan.FromMilliseconds(int.Parse(config["ExecutionOptions:RealTimeExecutionOptions:RequestCycleTimeMs"]!));

            public static TimeSpan PresentationCycleTime =>
                TimeSpan.FromMilliseconds(int.Parse(config["ExecutionOptions:RealTimeExecutionOptions:PresentationCycleTimeMs"]!));
        }

        public static class Iteration
        {
            public static int RequestsCount => int.Parse(config["ExecutionOptions:IterationExecutionOptions:RequestsCount"]!);
        }
    }

    public static class CacheOptions
    {
        public static CacheType CacheType => Enum.Parse<CacheType>(config["CacheOptions:CacheType"]!);

        public static bool Enabled => bool.Parse(config["CacheOptions:Enabled"]!);

        public static TimeSpan SlidingExpiration => TimeSpan.FromSeconds(int.Parse(config["CacheOptions:SlidingExpirationSeconds"]!));
    }

    public static class RepositoryOptions
    {
        public static RepositoryType RepositoryType => Enum.Parse<RepositoryType>(config["RepositoryOptions:RepositoryType"]!);
        
        public static TimeSpan ReadDelay =>
            TimeSpan.FromMilliseconds(int.Parse(config["RepositoryOptions:ReadDelayMs"]!));

        public static bool ClearDatabase => bool.Parse(config["RepositoryOptions:ClearDatabase"]!);
    }

    public static class ConnectionStrings
    {
        public static string Sqlite => config["ConnectionStrings:Sqlite"]!;
        
        public static string PostgreSql => config["ConnectionStrings:PostgreSQL"]!;

        public static string Redis => config["ConnectionStrings:Redis"]!;
    }
}