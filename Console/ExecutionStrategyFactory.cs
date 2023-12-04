using Core.Data;
using Core.Decorators;
using Core.ExecutionStrategy;
using Core.Metric;
using Core.Wrappers;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Console;

public static class ExecutionStrategyFactory
{
    public static async Task<ExecutionStrategy> CreateExecutionStrategy()
    {
        var dataRepository = await CreateDataRepository(Settings.RepositoryOptions.RepositoryType);

        await SeedData(dataRepository);

        var metricsRepository = new MetricsRepository();
        dataRepository = CreateDataRepositoryProxy(dataRepository, metricsRepository);

        var executionType = Settings.ExecutionOptions.ExecutionType;
        System.Console.WriteLine($"Creating ExecutionStrategy with type = {executionType}...");
        return executionType switch
        {
            ExecutionStrategyType.Iteration => new IterationExecutionStrategy(dataRepository,
                metricsRepository,
                new IterationExecutionOptions()
                {
                    DataCount = Settings.Seeding.DataCount,
                    RequestsCount = Settings.ExecutionOptions.Iteration.RequestsCount,
                    UpdateOperationProbable = Settings.ExecutionOptions.UpdateOperationProbable,
                    MaxBytesDataLength = Settings.Seeding.MaxBytesLength,
                    MaxStringDataLength = Settings.Seeding.MaxStringLength
                }),
            ExecutionStrategyType.RealTime => new RealTimeExecutionStrategy(dataRepository,
                metricsRepository,
                new RealTimeExecutionOptions()
                {
                    DataCount = Settings.Seeding.DataCount,
                    RequestCycleTime = Settings.ExecutionOptions.RealTime.RequestCycleTimeMs,
                    PresentationCycleTime = Settings.ExecutionOptions.RealTime.PresentationCycleTime,
                    UpdateOperationProbable = Settings.ExecutionOptions.UpdateOperationProbable,
                    MaxBytesDataLength = Settings.Seeding.MaxBytesLength,
                    MaxStringDataLength = Settings.Seeding.MaxStringLength
                }),
            _ => throw new ArgumentOutOfRangeException(nameof(executionType), executionType, null)
        };
    }

    private static IDataRepository CreateDataRepositoryProxy(IDataRepository dataRepository, MetricsRepository metricsRepository)
    {
        dataRepository = new DataRepositoryMetricsDecorator(dataRepository, metricsRepository, 
            new DataRepositoryMetricsDecoratorOptions()
            {
                ReadDelay = Settings.RepositoryOptions.ReadDelay
            });

        if (!Settings.CacheOptions.Enabled)
        {
            System.Console.WriteLine("Without cache...");
            return dataRepository;
        }

        System.Console.WriteLine("Using cache...");
        var cache = CreateCache(Settings.CacheOptions.CacheType);
        ICacheWrapper cacheWrapper = new CacheWrapper(cache, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = Settings.CacheOptions.SlidingExpiration
        }, metricsRepository);
        cacheWrapper = new CacheMetricsDecorator(cacheWrapper, metricsRepository);
        return new CachedDataRepositoryDecorator(dataRepository, cacheWrapper);
    }

    private static async Task<IDataRepository> CreateDataRepository(RepositoryType repositoryType)
    {
        System.Console.WriteLine($"Create {repositoryType} Repository...");
        switch (repositoryType)
        {
            case RepositoryType.InMemory:
                return new InMemoryDataRepository();
            case RepositoryType.SqLite or RepositoryType.PostgreSql:
                var dbContext = repositoryType == RepositoryType.PostgreSql
                ? (ApplicationContext)PostgresContextFactory.CreateDbContext()
                : (ApplicationContext)SqliteContextFactory.CreateDbContext();
                var database = dbContext.Database;
                if (Settings.RepositoryOptions.ClearDatabase)
                {
                    System.Console.WriteLine("Delete data from DB...");
                    await database.EnsureDeletedAsync();
                }
                System.Console.WriteLine("Migrate DB...");
                await database.MigrateAsync();
                return new DbDataRepository(dbContext);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static IDistributedCache CreateCache(CacheType cacheType)
    {
        switch (cacheType)
        {
            case CacheType.InMemory:
                return new MemoryDistributedCache(
                    new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
            case CacheType.Redis:
                var options = new ConfigurationOptions()
                {
                    EndPoints = { Settings.ConnectionStrings.Redis },
                    AllowAdmin = true
                };
                var muxer = ConnectionMultiplexer.Connect(options);
                var server = muxer.GetServer(Settings.ConnectionStrings.Redis);
                server.FlushDatabase();
                server.ConfigSet("save", "");
                return new RedisCache(new RedisCacheOptions()
                {
                    Configuration = Settings.ConnectionStrings.Redis,
                });
            default:
                throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
        }
    }

    private static async Task SeedData(IDataRepository dataRepository)
    {
        if (await dataRepository.Get(1) is not null) return;
        System.Console.WriteLine("Seed data...");
        var entries = Seed.GetData(new SeedOptions()
        {
            DataCount = Settings.Seeding.DataCount,
            MinStringLength = Settings.Seeding.MinStringLength,
            MaxStringLength = Settings.Seeding.MaxStringLength,
            MinBytesLength = Settings.Seeding.MinBytesLength,
            MaxBytesLength = Settings.Seeding.MaxBytesLength,
        });
        foreach (var entry in entries)
            await dataRepository.Create(entry);
    }
}