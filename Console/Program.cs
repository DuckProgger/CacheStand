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

internal class Program
{

    static async Task Main(string[] args)
    {
        var dbContext = ApplicationContextFactory.CreateDbContext();
        await dbContext.Database.MigrateAsync();
        var metrics = new Metrics();
        var metricsStorage = new MetricsStorage();
        var inMemoryRepository = new InMemoryRepository();
        var dbRepository = new DbRepository(dbContext);
        var cache = new MemoryDistributedCache(
            new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var cacheWrapper = new DistributedCacheWrapper(cache, new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromSeconds(10)
        });
        var cacheWrapperProxy = new DistributedCacheWrapperProxy(cacheWrapper, metrics);
        var repositoryProxyInner = new DistributedCacheRepositoryProxy(dbRepository, cacheWrapperProxy);
        var repositoryProxyOuter = new RequestTimeMeasurmentRepositoryProxy(repositoryProxyInner, metrics);

        await SeedData(dbRepository);
        var dataCount = Seed.DataCount;

        var executionStrategy = CreateExecutionStrategy(ExecutionStrategyType.RealTime,
            repositoryProxyOuter, metricsStorage, metrics, dataCount);
        var logPath = $"logs/{DateTime.Now:yy-MM-dd HH-mm}.txt";
        executionStrategy.ResultReceived += ShowResults;
        executionStrategy.ResultReceived += result => MetricsCsvWriter.Write(logPath, result);
        await executionStrategy.Invoke();

        System.Console.ReadKey();
    }

    private static ExecutionStrategy CreateExecutionStrategy(ExecutionStrategyType strategyType,
        IRepository repository, MetricsStorage metricsStorage, Metrics metrics, int dataCount)
    {
        return strategyType switch
        {
            ExecutionStrategyType.Iteration => new IterationExecutionStrategy(repository, metricsStorage,
                metrics,
                new IterationExecutionOptions()
                {
                    DataCount = dataCount,
                    RequestsCount = dataCount,
                    UpdateOperationProbable = 00
                }),
            ExecutionStrategyType.RealTime => new RealTimeExecutionStrategy(repository, metricsStorage,
                metrics,
                new RealTimeExecutionOptions()
                {
                    DataCount = dataCount,
                    RequestCycleTime = TimeSpan.FromMilliseconds(500),
                    PresentationCycleTime = TimeSpan.FromMilliseconds(1000),
                    UpdateOperationProbable = 20
                }),
            _ => throw new ArgumentOutOfRangeException(nameof(strategyType), strategyType, null)
        };
    }

    private static async Task SeedData(IRepository repository)
    {
        if (await repository.Get(1) is not null) return;
        var entries = Seed.GetData();
        foreach (var entry in entries)
            await repository.Create(entry);
    }

    private static void ShowResults(MetricsResult metricsResult)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Acc: {metricsResult.QueryAcceleration:.##} %
                                  HR: {metricsResult.QueryAcceleration:.##} %
                                  RPS: {metricsResult.RequestsPerSecond}
                                  Total requests: {metricsResult.TotalRequests}
                                  Total read requests: {metricsResult.TotalReadRequests}
                                  Total hits: {metricsResult.TotalCacheHits}
                                  """);
    }
}