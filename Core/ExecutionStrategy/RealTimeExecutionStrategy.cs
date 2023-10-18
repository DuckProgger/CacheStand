using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public class RealTimeExecutionStrategy : ExecutionStrategyBase
{
    private readonly Action<MetricsResult> consumeResultsAction;
    private readonly RealTimeExecutionOptions options;

    public RealTimeExecutionStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
        Action<MetricsResult> consumeResultsAction,
        RealTimeExecutionOptions options) : base(repository, metricsStorage, metrics, options)
    {
        this.consumeResultsAction = consumeResultsAction;
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestSimulationTimer = new SingleThreadTimer(SimulateRequest, options.RequestCycleTime);

        var consumerTimer = new SingleThreadTimer(() =>
        {
            var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
            var metricsResult = metricsCalc.Calculate(ExecutionStrategyType.RealTime);
            consumeResultsAction(metricsResult);
            return Task.CompletedTask;
        }, options.PresentationCycleTime);

        await requestSimulationTimer.Start();
        await consumerTimer.Start();
    }
}