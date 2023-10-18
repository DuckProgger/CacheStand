using Core.Metric;

namespace Console;

internal class MetricsCsvWriter
{
    public static void Write(string path, MetricsResult data)
    {
        var directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath!);
        var strData =
            $"{data.QueryAcceleration};{data.HitRate};{data.RequestsPerSecond};{data.TotalRequests};" +
            $"{data.TotalReadRequests};{data.TotalCacheHits}{Environment.NewLine}";
        File.AppendAllText(path, strData);
    }
}
