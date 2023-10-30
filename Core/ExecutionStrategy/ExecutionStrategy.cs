using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public abstract class ExecutionStrategy
{
    private readonly IRepository repository;
    private readonly MetricsWriter metricsWriter;
    private readonly ExecutionOptions options;

    protected readonly MetricsStorage metricsStorage;

    protected ExecutionStrategy(IRepository repository,
        MetricsStorage metricsStorage, MetricsWriter metricsWriter, ExecutionOptions options)
    {
        this.repository = repository;
        this.metricsStorage = metricsStorage;
        this.metricsWriter = metricsWriter;
        this.options = options;
    }

    public event Action<MetricsResult>? ResultReceived;

    public abstract Task Invoke();
    
    protected async Task SimulateRequest()
    {
        var updateOperation = Randomizer.GetProbableEvent(options.UpdateOperationProbable);
        if (updateOperation)
            await UpdateOperation();
        else
            await ReadOperation();
    }
    
    private async Task UpdateOperation()
    {
        var randomId = Randomizer.GetRandomInt(1, options.DataCount);
        var newEntry = new Entry()
        {
            Id = randomId,
            Data = Randomizer.GetRandomBytes(10000),
            Text = Randomizer.GetRandomString(10000)
        };
        metricsWriter.Begin();
        metricsWriter.WriteIsReadOperation(false);
        await repository.Update(newEntry);
        metricsStorage.Add(metricsWriter.EndAndGet());
    }

    private async Task ReadOperation()
    {
        var randomId = Randomizer.GetRandomInt(1, options.DataCount);
        metricsWriter.Begin();
        metricsWriter.WriteIsReadOperation(true);
        var entry = await repository.Get(randomId);
        metricsStorage.Add(metricsWriter.EndAndGet());
    }

    protected virtual void OnResultReceived(MetricsResult result)
    {
        ResultReceived?.Invoke(result);
    }
}