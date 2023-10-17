using Core.Data;

namespace Infrastructure;
public static class Seed
{
    public const int DataCount = 1000;
    
    private const int minStringLength = 1000;
    private const int maxStringLength = 10000;
    private const int minBytesLength = 10000;
    private const int maxBytesLength = 100000;

    public static IEnumerable<Entry> GetData()
    {
        return Enumerable.Range(0, DataCount)
            .Select(_ => new Entry()
            {
                Text = Randomizer.GetRandomString(minStringLength, maxStringLength),
                Data = Randomizer.GetRandomBytes(minBytesLength, maxBytesLength)
            });
    }
}
