using Core.Data;
using Core.Metric;
using Infrastructure;

namespace Console.ExecutionStrategy;

abstract class ExecuteStrategyBase : IExecuteStrategy
{
    private readonly IRepository repository;
    private readonly Metrics metrics;
    private readonly ExecutionOptions options;

    protected readonly MetricsStorage metricsStorage;

    protected ExecuteStrategyBase(IRepository repository,
        MetricsStorage metricsStorage, Metrics metrics, ExecutionOptions options)
    {
        this.repository = repository;
        this.metricsStorage = metricsStorage;
        this.metrics = metrics;
        this.options = options;
    }

    public abstract Task Invoke();

    protected async Task UpdateOperation()
    {
        var randomId = Random.Shared.Next(1, options.DataCount);
        var newEntry = new Entry()
        {
            Id = randomId,
            Data = Randomizer.GetRandomBytes(10000),
            Text = Randomizer.GetRandomString(10000)
        };
        await repository.Update(newEntry);
        metricsStorage.Add(metrics with { });
        metrics.Clear();
    }

    protected async Task ReadOperation()
    {
        var randomId = Random.Shared.Next(1, options.DataCount);
        var entry = await repository.Get(randomId);
        metricsStorage.Add(metrics with { });
        metrics.Clear();
    }

    protected static void ShowResults(MetricsCalc metricsCalc)
    {
        System.Console.Clear();
        System.Console.WriteLine($"""
                                  Acc: {metricsCalc.GetQueryAcceleration():.##} %
                                  HR: {metricsCalc.GetHitRate():.##} %
                                  RPS: {metricsCalc.GetRps()}
                                  Total requests: {metricsCalc.GetTotalRequests()}
                                  Total hits: {metricsCalc.GetTotalCacheHits()}
                                  """);
    }
}