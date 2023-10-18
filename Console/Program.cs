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
        var executionStrategy = await ExecutionStrategyFactory.CreateExecutionStrategy();
        var logPath = $"logs/{DateTime.Now:yy_MM_dd HH-mm-ss}.csv";
        executionStrategy.ResultReceived += ShowResults;
        executionStrategy.ResultReceived += result => MetricsCsvWriter.Write(logPath, result);
        await executionStrategy.Invoke();

        System.Console.ReadKey();
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