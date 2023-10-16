using Core.Data;
using Core.Metric;
using Core.Services;
using Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        var metrics = new Metrics();
        var metricsStorage = new MetricsStorage();
        var inMemoryRepository = new InMemoryRepository();
        var cache = new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions())); 
        var cacheWrapper = new DistributedCacheWrapper(cache);
        var cacheWrapperProxy = new DistributedCacheWrapperProxy(cacheWrapper, metrics);
        var repositoryProxyInner = new DistributedCacheRepositoryProxy(inMemoryRepository, cacheWrapperProxy);
        var repositoryProxyOuter = new RequestTimeMeasurmentRepositoryProxy(repositoryProxyInner, metrics);

        await FeelSeedData(repositoryProxyInner);
        var dataCount = Seed.DataCount;
        var requestsCount = dataCount * 10;
        for (int i = 0; i < requestsCount; i++)
        {
            var randomId = Random.Shared.Next(1, dataCount);
            var entry = await repositoryProxyOuter.Get(randomId);
            metricsStorage.Add(metrics with {});
            metrics.Clear();
        }

        var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
        ShowResults(metricsCalc);
    }

    private static void ShowResults(MetricsCalc metricsCalc)
    {
        System.Console.WriteLine($"Acc: {metricsCalc.GetQueryAcceleration():.##} %; HR: {metricsCalc.GetHitRate()}; RPS: {metricsCalc.GetRps()}");
    }

    static async Task FeelSeedData(IRepository repository)
    {
        var entries = Seed.GetData();
        foreach (var entry in entries)
            await repository.Create(entry);
    }
}
