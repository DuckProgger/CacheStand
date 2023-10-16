namespace Infrastructure;

public static class Randomizer
{
    private static readonly Random random = Random.Shared;
    private const int startCharIndex = 'A';
    private const int endCharIndex = 'z';

    public static string GetRandomString(int minLength, int maxLength)
    {
        var length = random.Next(minLength, maxLength);
        var randomChars = Enumerable.Range(0, length)
            .Select(_ => (char)random.Next(startCharIndex, endCharIndex))
            .ToArray();
        return new string(randomChars);
    }

    public static string GetRandomString(int maxLength)
    {
        return GetRandomString(0, maxLength);
    }

    public static byte[] GetRandomBytes(int minLength, int maxLength)
    {
        var length = random.Next(minLength, maxLength);
        var buffer = new byte[length];
        random.NextBytes(buffer);
        return buffer;
    }

    public static byte[] GetRandomBytes(int maxLength)
    {
        return GetRandomBytes(0, maxLength);
    }

    public static bool GetProbableEvent(int probability)
    {
        if (probability >= 100) return true;
        if (probability <= 0) return false;
        var randomValue = random.Next(0, 100);
        return probability >= randomValue;
    }
}
