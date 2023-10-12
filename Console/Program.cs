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

        await cacheDec.Create(new Entry()
        {
            Text = "test",
            Data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }
        });

        for (int i = 0; i < 500; i++)
        {
            var entry = cacheDec.Get(1);
            await Task.Delay(10);
        }

        var mc = new MetricsCalc(ms.GetAll());
        System.Console.WriteLine($"{mc.GetQueryAcceleration()}; {mc.GetHitRate()}; {mc.GetRps()}");
    }
}
