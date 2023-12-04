using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public class RealTimeExecutionStrategy : ExecutionStrategy
{
    private readonly RealTimeExecutionOptions options;

    public RealTimeExecutionStrategy(IDataRepository dataRepository, MetricsRepository metricsRepository,
        RealTimeExecutionOptions options) : base(dataRepository, metricsRepository, options)
    {
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestSimulationTimer = new SingleThreadTimer(SimulateRequest, options.RequestCycleTime);

        var consumerTimer = new SingleThreadTimer(() =>
        {
            var metricsCalc = new MetricsCalc(metricsRepository.GetMetrics());
            var metricsResult = metricsCalc.Calculate(ExecutionStrategyType.RealTime);
            OnResultReceived(metricsResult);
            return Task.CompletedTask;
        }, options.PresentationCycleTime);

        var cts = new CancellationTokenSource();
        var simulationTask = requestSimulationTimer.Start(cts.Token);
        var consumerTask = consumerTimer.Start(cts.Token);
        var completedTask = await Task.WhenAny(simulationTask, consumerTask);
        await completedTask;
    }
}