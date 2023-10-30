namespace Core.ExecutionStrategy;

public class ExecutionOptions
{
    public int DataCount { get; set; }
    
    public int UpdateOperationProbable { get; set; }
    
    public int MaxStringDataLength { get; set; }
    
    public int MaxBytesDataLength { get; set; }
}