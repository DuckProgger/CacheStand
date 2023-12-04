using System.Diagnostics;
using Console.Writers;

namespace Console;

internal class Program
{
    static async Task Main(string[] args)
    {
        var executionStrategy = await ExecutionStrategyFactory.CreateExecutionStrategy();
        var metricsConsoleWriter = new MetricsConsoleWriter();
        var logPath = $"logs/{DateTime.Now:yy_MM_dd HH-mm-ss}.csv";
        var metricsCsvWriter = new MetricsCsvWriter(logPath);
        executionStrategy.ResultReceived += metricsConsoleWriter.Write;
        executionStrategy.ResultReceived += metricsCsvWriter.Write;
        System.Console.WriteLine("Starting requests...");
        await executionStrategy.Invoke();

        System.Console.ReadKey();
    }
}