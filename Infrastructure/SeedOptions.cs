namespace Infrastructure;

public class SeedOptions
{
    public int DataCount { get; set; }
    
    public int MinStringLength { get; set; }
    
    public int MaxStringLength { get; set; }
    
    public int MinBytesLength { get; set; }
    
    public int MaxBytesLength { get; set; }
}