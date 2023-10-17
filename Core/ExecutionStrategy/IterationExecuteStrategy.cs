using Core.Data;
using Core.Metric;

namespace Core.ExecutionStrategy;

public class IterationExecuteStrategy : ExecuteStrategyBase
{
    private readonly Action<MetricsCalc> consumeResultsAction;
    private readonly IterationExecutionOptions options;

    public IterationExecuteStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
        Action<MetricsCalc> consumeResultsAction,
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
        consumeResultsAction(metricsCalc);
    }
}