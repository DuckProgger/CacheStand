using Core.Data;
using Core.Metric;
using Core.Utils;
using Infrastructure;

namespace Console.ExecutionStrategy;

class RealTimeExecuteStrategy : ExecuteStrategyBase
{
    private readonly RealTimeExecutionOptions options;

    public RealTimeExecuteStrategy(IRepository repository,
        MetricsStorage metricsStorage,
        Metrics metrics,
        RealTimeExecutionOptions options) : base(repository, metricsStorage, metrics, options)
    {
        this.options = options;
    }

    public override async Task Invoke()
    {
        var requestSimulationTimer = new SingleThreadTimer(async () =>
        {
            var updateOperation = Randomizer.GetProbableEvent(options.UpdateOperationProbable);
            if (updateOperation)
                await UpdateOperation();
            else
                await ReadOperation();
        }, options.RequestCycleTime);

        var presenterTimer = new SingleThreadTimer(() =>
        {
            var metricsCalc = new MetricsCalc(metricsStorage.GetAll());
            ShowResults(metricsCalc);
            return Task.CompletedTask;
        }, options.PresentationCycleTime);

        await requestSimulationTimer.Start();
        await presenterTimer.Start();
    }
}