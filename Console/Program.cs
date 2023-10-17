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
            SlidingExpiration = TimeSpan.FromSeconds(100)
        });
        var cacheWrapperProxy = new DistributedCacheWrapperProxy(cacheWrapper, metrics);
        var repositoryProxyInner = new DistributedCacheRepositoryProxy(dbRepository, cacheWrapperProxy);
        var repositoryProxyOuter = new RequestTimeMeasurmentRepositoryProxy(repositoryProxyInner, metrics);

        await SeedData(dbRepository);
        var dataCount = Seed.DataCount;

        //var executionStrategy = new RealTimeExecuteStrategy(repositoryProxyOuter, metricsStorage, metrics,
        //    ShowRealTimeResults,
        //    new RealTimeExecutionOptions()
        //    {
        //        DataCount = dataCount,
        //        RequestCycleTime = TimeSpan.FromMilliseconds(20),
        //        PresentationCycleTime = TimeSpan.FromMilliseconds(1000),
        //        UpdateOperationProbable = 20
        //    });
        var executionStrategy = new IterationExecuteStrategy(repositoryProxyOuter, metricsStorage, metrics,
            ShowIterationResults,
            new IterationExecutionOptions()
            {
                DataCount = dataCount,
                RequestsCount = dataCount,
                UpdateOperationProbable = 50
            });
        await executionStrategy.Invoke();

        System.Console.ReadKey();
    }

    private static async Task SeedData(IRepository repository)
    {
        if (await repository.Get(1) is not null) return;
        var entries = Seed.GetData();
        foreach (var entry in entries)
            await repository.Create(entry);
    }

    protected static void ShowRealTimeResults(MetricsCalc metricsCalc)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Acc: {metricsCalc.GetQueryAcceleration():.##} %
                                  HR: {metricsCalc.GetHitRate():.##} %
                                  RPS: {metricsCalc.GetLastRps()}
                                  Total requests: {metricsCalc.GetTotalRequests()}
                                  Total read requests: {metricsCalc.GetTotalReadRequests()}
                                  Total hits: {metricsCalc.GetTotalCacheHits()}
                                  """);
    }
    
    protected static void ShowIterationResults(MetricsCalc metricsCalc)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Acc: {metricsCalc.GetQueryAcceleration():.##} %
                                  HR: {metricsCalc.GetHitRate():.##} %
                                  Average RPS: {metricsCalc.GetAverageRps()}
                                  Total requests: {metricsCalc.GetTotalRequests()}
                                  Total read requests: {metricsCalc.GetTotalReadRequests()}
                                  Total hits: {metricsCalc.GetTotalCacheHits()}
                                  """);
    }
}