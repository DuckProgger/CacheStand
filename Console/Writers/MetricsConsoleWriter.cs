using Core.Metric;

namespace Console.Writers;

public class MetricsConsoleWriter : IMetricsWriter
{
    public void Write(MetricsResult metricsResult)
    {
        System.Console.Clear();

        var percantageMetricsColor = ConsoleColor.DarkYellow;
        var numberMetricsColor = ConsoleColor.DarkGreen;
        var timeMetricsColor = ConsoleColor.Cyan;

        var tableConsoleWriter = new ConsoleTableConsoleWriter(
            new ConsoleTableConsoleWriter.Column("Parameter", 35),
            new ConsoleTableConsoleWriter.Column("Value", 20));
        
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Cache efficiency", percantageMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.CacheEfficiency:.##} %", percantageMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Hit rate", percantageMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.HitRate:.##} %", percantageMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("RPS", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.RequestsPerSecond, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total requests", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequests, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total read requests", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalReadRequests, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total hits", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalCacheHits, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total request time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequestTime, timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average request time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRequestTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache costs", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageCacheTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average repository read time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryReadTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache read time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageReadCacheTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average deserialization time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average repository write time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryWriteTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache write time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageWriteCacheTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average serialization time", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageSerializationTime.TotalMicroseconds} мкс", timeMetricsColor));

        tableConsoleWriter.Write();
    }
}