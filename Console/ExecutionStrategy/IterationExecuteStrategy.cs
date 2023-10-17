using Core.Data;
using Core.Metric;

namespace Console.ExecutionStrategy;

class IterationExecuteStrategy : ExecuteStrategyBase
{
    private readonly IterationExecutionOptions options;

    public IterationExecuteStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
        IterationExecutionOptions options) : base(repository, metricsStorage, metrics, options)
    {
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestsCount = options.RequestsCount;
        for (int i = 0; i < requestsCount; i++)
            await SimulateRequest();
        var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
        ShowResults(metricsCalc);
    }
}