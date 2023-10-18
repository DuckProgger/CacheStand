using Core.Data;
using Core.ExecutionStrategy;
using Core.Metric;
using Core.Proxies;
using Core.Wrappers;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Console;

public static class ExecutionStrategyFactory
{
    public static async Task<ExecutionStrategy> CreateExecutionStrategy()
    {
        var repository = await CreateRespository(Settings.RepositoryType);
        await SeedData(repository);

        var metrics = new Metrics();
        var metricsStorage = new MetricsStorage();

        var cache = new MemoryDistributedCache(
            new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var cacheWrapper = new DistributedCacheWrapper(cache, new DistributedCacheEntryOptions()
        {
            SlidingExpiration = Settings.CacheOptions.SlidingExpiration
        });
        var cacheWrapperProxy = new DistributedCacheWrapperProxy(cacheWrapper, metrics);

        var repositoryProxyInner = new DistributedCacheRepositoryProxy(repository, cacheWrapperProxy);
        var repositoryProxyOuter = new RequestTimeMeasurmentRepositoryProxy(repositoryProxyInner, metrics);

        var executionType = Settings.ExecutionOptions.ExecutionType;
        return executionType switch
        {
            ExecutionStrategyType.Iteration => new IterationExecutionStrategy(repositoryProxyOuter, metricsStorage,
                metrics,
                new IterationExecutionOptions()
                {
                    DataCount = Settings.Seeding.DataCount,
                    RequestsCount = Settings.ExecutionOptions.Iteration.RequestsCount,
                    UpdateOperationProbable = Settings.ExecutionOptions.UpdateOperationProbable
                }),
            ExecutionStrategyType.RealTime => new RealTimeExecutionStrategy(repositoryProxyOuter, metricsStorage,
                metrics,
                new RealTimeExecutionOptions()
                {
                    DataCount = Settings.Seeding.DataCount,
                    RequestCycleTime = Settings.ExecutionOptions.RealTime.RequestCycleTimeMs,
                    PresentationCycleTime = Settings.ExecutionOptions.RealTime.PresentationCycleTime,
                    UpdateOperationProbable = Settings.ExecutionOptions.UpdateOperationProbable
                }),
            _ => throw new ArgumentOutOfRangeException(nameof(executionType), executionType, null)
        };
    }

    private static async Task<IRepository> CreateRespository(RepositoryType repositoryType)
    {
        switch (repositoryType)
        {
            case RepositoryType.InMemory:
                return new InMemoryRepository();
            case RepositoryType.SqLite:
                var dbContext = ApplicationContextFactory.CreateDbContext();
                await dbContext.Database.MigrateAsync();
                return new DbRepository(dbContext);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static async Task SeedData(IRepository repository)
    {
        if (await repository.Get(1) is not null) return;
        var entries = Seed.GetData(new SeedOptions()
        {
            DataCount = Settings.Seeding.DataCount,
            MinStringLength = Settings.Seeding.MinStringLength,
            MaxStringLength = Settings.Seeding.MaxStringLength,
            MinBytesLength = Settings.Seeding.MinBytesLength,
            MaxBytesLength = Settings.Seeding.MaxBytesLength,
        });
        foreach (var entry in entries)
            await repository.Create(entry);
    }
}