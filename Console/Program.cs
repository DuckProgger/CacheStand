using Core.Data;
using Core.ExecutionStrategy;
using Core.Metric;
using Core.Services;
using Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Console;

internal class Program
{
    [STAThread]
    static async Task Main(string[] args)
    {
        var metrics = new Metrics();
        var metricsStorage = new MetricsStorage();
        var inMemoryRepository = new InMemoryRepository();
        var cache = new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions()));
        var cacheWrapper = new DistributedCacheWrapper(cache, new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromSeconds(100)
        });
        var cacheWrapperProxy = new DistributedCacheWrapperProxy(cacheWrapper, metrics);
        var repositoryProxyInner = new DistributedCacheRepositoryProxy(inMemoryRepository, cacheWrapperProxy);
        var repositoryProxyOuter = new RequestTimeMeasurmentRepositoryProxy(repositoryProxyInner, metrics);

        await FeelSeedData(inMemoryRepository);
        var dataCount = Seed.DataCount;

        //var executionStrategy = new RealTimeExecuteStrategy(repositoryProxyOuter, metricsStorage, metrics,
        //    ShowResults,
        //    new RealTimeExecutionOptions()
        //    {
        //        DataCount = dataCount,
        //        RequestCycleTime = TimeSpan.FromMilliseconds(10),
        //        PresentationCycleTime = TimeSpan.FromMilliseconds(1000),
        //        UpdateOperationProbable = 20
        //    });
        var executionStrategy = new IterationExecuteStrategy(repositoryProxyOuter, metricsStorage, metrics,
            ShowResults,
            new IterationExecutionOptions()
            {
                DataCount = Seed.DataCount,
                RequestsCount = dataCount,
                UpdateOperationProbable = 50
            });
        await executionStrategy.Invoke();

        System.Console.ReadKey();
    }

    static async Task FeelSeedData(IRepository repository)
    {
        var entries = Seed.GetData();
        foreach (var entry in entries)
            await repository.Create(entry);
    }

    protected static void ShowResults(MetricsCalc metricsCalc)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Acc: {metricsCalc.GetQueryAcceleration():.##} %
                                  HR: {metricsCalc.GetHitRate():.##} %
                                  RPS: {metricsCalc.GetRps()}
                                  Total requests: {metricsCalc.GetTotalRequests()}
                                  Total read requests: {metricsCalc.GetTotalReadRequests()}
                                  Total hits: {metricsCalc.GetTotalCacheHits()}
                                  """);
    }
}