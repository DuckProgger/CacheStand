using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public class RealTimeExecuteStrategy : ExecuteStrategyBase
{
    private readonly Action<MetricsCalc> consumeResultsAction;
    private readonly RealTimeExecutionOptions options;

    public RealTimeExecuteStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
        Action<MetricsCalc> consumeResultsAction,
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
            consumeResultsAction(metricsCalc);
            return Task.CompletedTask;
        }, options.PresentationCycleTime);

        await requestSimulationTimer.Start();
        await consumerTimer.Start();
    }
}