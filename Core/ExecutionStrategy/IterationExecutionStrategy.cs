using Core.Data;
using Core.Metric;

namespace Core.ExecutionStrategy;

public class IterationExecutionStrategy : ExecutionStrategy
{
    private readonly IterationExecutionOptions options;

    public IterationExecutionStrategy(IDataRepository dataRepository, MetricsRepository metricsRepository,
        IterationExecutionOptions options) : base(dataRepository, metricsRepository, options)
    {
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestsCount = options.RequestsCount;
        for (int i = 0; i < requestsCount; i++)
            await SimulateRequest();
        var metricsCalc = new MetricsCalc(metricsRepository.GetMetrics());
        var metricsResult = metricsCalc.Calculate(ExecutionStrategyType.Iteration);
        OnResultReceived(metricsResult);
    }
}