using Core.Metric;

namespace Console.Writers;

public class MetricsConsoleWriter : IMetricsWriter
{
    public void Write(MetricsResult metricsResult)
    {
        System.Console.Clear();

        var test = new ConsoleTableConsoleWriter(
            new ConsoleTableConsoleWriter.Column("Parameter", 35),
            new ConsoleTableConsoleWriter.Column("Value", 20));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total request time"),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequestTime));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average request time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRequestTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache costs"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRequestTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Cache efficiency"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.CacheEfficiency:.##} %"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Hit rate"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.HitRate:.##} %"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("RPS"),
            new ConsoleTableConsoleWriter.Cell(metricsResult.RequestsPerSecond));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total requests"),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequests));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total read requests"),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalReadRequests));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total hits"),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalCacheHits));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average repository read time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryReadTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache read time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageReadCacheTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average deserialization time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average repository write time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryWriteTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache write time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageWriteCacheTime.TotalMicroseconds} мкс"));
        test.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average serialization time"),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageSerializationTime.TotalMicroseconds} мкс"));

        test.Write();
        
        
        //System.Console.WriteLine($"""
        //                          Total request time:...{metricsResult.TotalRequestTime}
        //                          Average request time:.{metricsResult.AverageRequestTime.TotalMicroseconds} мкс
        //                          Average cache costs:..{metricsResult.AverageCacheTime.TotalMicroseconds} мкс
        //                          Cache efficiency:.....{metricsResult.CacheEfficiency:.##} %
        //                          Hit rate:.............{metricsResult.HitRate:.##} %
        //                          RPS:..................{metricsResult.RequestsPerSecond}
        //                          Total requests:.......{metricsResult.TotalRequests}
        //                          Total read requests:..{metricsResult.TotalReadRequests}
        //                          Total hits:...........{metricsResult.TotalCacheHits}
        //                          """);
        //System.Console.WriteLine();
        //System.Console.WriteLine($"""
        //                          Average repository read time:..{metricsResult.AverageRepositoryReadTime.TotalMicroseconds} мкс
        //                          Average cache read time:.......{metricsResult.AverageReadCacheTime.TotalMicroseconds} мкс
        //                          Average deserialization time:..{metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс
        //                          Average repository write time:.{metricsResult.AverageRepositoryWriteTime.TotalMicroseconds} мкс
        //                          Average cache write time:......{metricsResult.AverageWriteCacheTime.TotalMicroseconds} мкс
        //                          Average serialization time:....{metricsResult.AverageSerializationTime.TotalMicroseconds} мкс
        //                          """);
    }
}