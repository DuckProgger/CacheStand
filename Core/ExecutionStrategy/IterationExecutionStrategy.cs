using Core.Data;
using Core.Metric;

namespace Core.ExecutionStrategy;

public class IterationExecutionStrategy : ExecutionStrategyBase
{
    private readonly Action<MetricsResult> consumeResultsAction;
    private readonly IterationExecutionOptions options;

    public IterationExecutionStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
        Action<MetricsResult> consumeResultsAction,
        IterationExecutionOptions options) : base(repository, metricsStorage, metrics, options)
    {
        this.consumeResultsAction = consumeResultsAction;
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestsCount = options.RequestsCount;
        for (int i = 0; i < requestsCount; i++)
            await SimulateRequest();
        var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
        var metricsResult = metricsCalc.Calculate(ExecutionStrategyType.Iteration);
        consumeResultsAction(metricsResult);
    }
}