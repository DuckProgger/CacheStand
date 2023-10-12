using Core.Data;
using Core.Metric;
using Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        var ms = new MetricsStorage();
        var rep = new InMemoryRepository();
        var cacheDec = new DistributedCacheRepositoryDecorator(rep,
            new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(new())), ms);

        await FeelSeedData(cacheDec);
        var dataCount = Seed.DataCount;

        var requestsCount = dataCount * 10;
        for (int i = 0; i < requestsCount; i++)
        {
            var randomId = Random.Shared.Next(1, dataCount);
            var entry = cacheDec.Get(randomId);
        }

        var mc = new MetricsCalc(ms.GetAll());
        System.Console.WriteLine($"Acc: {mc.GetQueryAcceleration():.##} %; HR: {mc.GetHitRate()}; RPS: {mc.GetRps()}");
    }

    static async Task FeelSeedData(IRepository repository)
    {
        var entries = Seed.GetData();
        foreach (var entry in entries)
            await repository.Create(entry);
    }
}
