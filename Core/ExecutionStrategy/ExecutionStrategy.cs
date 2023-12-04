using Core.Data;
using Core.Metric;
using Core.Utils;

namespace Core.ExecutionStrategy;

public abstract class ExecutionStrategy
{
    private readonly IDataRepository dataRepository;
    private readonly ExecutionOptions options;

    protected readonly MetricsRepository metricsRepository;

    protected ExecutionStrategy(IDataRepository dataRepository, MetricsRepository metricsRepository, ExecutionOptions options)
    {
        this.dataRepository = dataRepository;
        this.metricsRepository = metricsRepository;
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
            Data = Randomizer.GetRandomBytes(options.MaxBytesDataLength),
            Text = Randomizer.GetRandomString(options.MaxStringDataLength)
        };
        metricsRepository.StartNew();
        metricsRepository.WriteIsReadOperation(false);
        await dataRepository.Update(newEntry);
        metricsRepository.EndWrite();
    }

    private async Task ReadOperation()
    {
        var randomId = Randomizer.GetRandomInt(1, options.DataCount);
        metricsRepository.StartNew();
        metricsRepository.WriteIsReadOperation(true);
        var entry = await dataRepository.Get(randomId);
        metricsRepository.EndWrite();
    }

    protected virtual void OnResultReceived(MetricsResult result)
    {
        ResultReceived?.Invoke(result);
    }
}