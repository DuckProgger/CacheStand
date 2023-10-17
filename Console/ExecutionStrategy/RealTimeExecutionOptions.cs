namespace Console.ExecutionStrategy;

public class RealTimeExecutionOptions : ExecutionOptions
{
    public TimeSpan RequestCycleTime { get; set; }

    public TimeSpan PresentationCycleTime { get; set; }

    public int UpdateOperationProbable { get; set; }
}