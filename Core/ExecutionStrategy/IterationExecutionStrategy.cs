using Core.Data;
using Core.Metric;

namespace Core.ExecutionStrategy;

public class IterationExecutionStrategy : ExecutionStrategy
{
    private readonly IterationExecutionOptions options;

    public IterationExecutionStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        MetricsWriter metricsWriter,
        IterationExecutionOptions options) : base(repository, metricsStorage, metricsWriter, options)
    {
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestsCount = options.RequestsCount;
        for (int i = 0; i < requestsCount; i++)
            await SimulateRequest();
        var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
        var metricsResult = metricsCalc.Calculate(ExecutionStrategyType.Iteration);
        OnResultReceived(metricsResult);
    }
}