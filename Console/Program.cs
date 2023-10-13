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
        var cacheWrapperDecorator = new DistributedCacheWrapperDecorator(cacheWrapper, metrics);
        var repositoryDecoratorInner = new DistributedCacheRepositoryDecorator(inMemoryRepository, cacheWrapperDecorator);
        var repositoryDecoratorOuter = new RequestTimeMeasurmentRepositoryDecorator(repositoryDecoratorInner, metrics);

        await FeelSeedData(repositoryDecoratorInner);
        var dataCount = Seed.DataCount;
        var requestsCount = dataCount * 10;
        for (int i = 0; i < requestsCount; i++)
        {
            var randomId = Random.Shared.Next(1, dataCount);
            var entry = await repositoryDecoratorOuter.Get(randomId);
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
