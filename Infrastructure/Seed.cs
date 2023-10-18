using Core.Data;
using Core.Utils;

namespace Infrastructure;

public static class Seed
{
    public static IEnumerable<Entry> GetData(SeedOptions options)
    {
        return Enumerable.Range(1, options.DataCount)
            .Select(id => new Entry()
            {
                Id = id,
                Text = Randomizer.GetRandomString(options.MinStringLength, options.MaxStringLength),
                Data = Randomizer.GetRandomBytes(options.MinBytesLength, options.MaxBytesLength)
            });
    }
}