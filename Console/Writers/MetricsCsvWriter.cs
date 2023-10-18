﻿using Core.Metric;

namespace Console.Writers;

internal class MetricsCsvWriter
{
    private readonly string path;

    public MetricsCsvWriter(string path)
    {
        this.path = path;
        CreateFile();
        WriteHeader();
    }

    public void Write(MetricsResult data)
    {
        var strData =
            $"{data.QueryAcceleration};" +
            $"{data.HitRate};" +
            $"{data.RequestsPerSecond};" +
            $"{data.TotalRequests};" +
            $"{data.TotalReadRequests};" +
            $"{data.TotalCacheHits}" +
            $"{Environment.NewLine}";
        File.AppendAllText(path, strData);
    }

    private void WriteHeader()
    {
        var strHeader =
        $"{nameof(MetricsResult.QueryAcceleration)};" +
        $"{nameof(MetricsResult.HitRate)};" +
        $"{nameof(MetricsResult.RequestsPerSecond)};" +
        $"{nameof(MetricsResult.TotalRequests)};" +
        $"{nameof(MetricsResult.TotalReadRequests)};" +
        $"{nameof(MetricsResult.TotalCacheHits)}" +
        $"{Environment.NewLine}";
        File.AppendAllText(path, strHeader);
    }

    private void CreateFile()
    {
        var directoryPath = Path.GetDirectoryName(path);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath!);
    }
}