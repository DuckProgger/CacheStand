using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public class RealTimeExecutionStrategy : ExecutionStrategy
{
    private readonly RealTimeExecutionOptions options;

    public RealTimeExecutionStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        MetricsWriter metrics,
        RealTimeExecutionOptions options) : base(repository, metricsStorage, metrics, options)
    {
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestSimulationTimer = new SingleThreadTimer(SimulateRequest, options.RequestCycleTime);

        var consumerTimer = new SingleThreadTimer(() =>
        {
            var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
            var metricsResult = metricsCalc.Calculate(ExecutionStrategyType.RealTime);
            OnResultReceived(metricsResult);
            return Task.CompletedTask;
        }, options.PresentationCycleTime);

        var cts = new CancellationTokenSource();
        var simulationTask = requestSimulationTimer.Start(cts.Token).CancelOnFaulted(cts);
        var consumerTask = consumerTimer.Start(cts.Token).CancelOnFaulted(cts);
        await Task.WhenAll(simulationTask, consumerTask);
    }
}