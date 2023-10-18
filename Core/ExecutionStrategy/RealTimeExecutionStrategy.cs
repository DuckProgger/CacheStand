using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public class RealTimeExecutionStrategy : ExecutionStrategy
{
    private readonly RealTimeExecutionOptions options;

    public RealTimeExecutionStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
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

        var simulationTask = requestSimulationTimer.Start();
        var consumerTask = consumerTimer.Start();
        await Task.WhenAll(simulationTask, consumerTask);
    }
}