using Core.Metric;
using System.Text;

namespace Console.Writers;

public class MetricsConsoleWriter : IMetricsWriter
{
    public void Write(MetricsResult metricsResult)
    {
        //System.Console.SetCursorPosition(0, 0);
        //System.Console.SetWindowSize(1500, 1000);
        System.Console.Clear();
        //System.Console.WriteLine("\u001b[2J\u001b[3J");

        //StringBuilder x = new StringBuilder().Append(' ', System.Console.WindowHeight * System.Console.WindowWidth);
        //System.Console.Write('\n' + x.ToString()); 
        //System.Console.SetCursorPosition(0, 0);

        //for (int i = 0; i < System.Console.BufferHeight; i++)
        //    System.Console.WriteLine();

        var tableConsoleWriter = new ConsoleTableConsoleWriter(
            new ConsoleTableConsoleWriter.Column("Parameter", 35),
            new ConsoleTableConsoleWriter.Column("Value", 20));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Cache efficiency", ConsoleColor.DarkYellow),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.CacheEfficiency:.##} %", ConsoleColor.DarkYellow));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Hit rate", ConsoleColor.DarkYellow),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.HitRate:.##} %", ConsoleColor.DarkYellow));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("RPS", ConsoleColor.DarkGreen),
            new ConsoleTableConsoleWriter.Cell(metricsResult.RequestsPerSecond, ConsoleColor.DarkGreen));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total requests", ConsoleColor.DarkGreen),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequests, ConsoleColor.DarkGreen));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total read requests", ConsoleColor.DarkGreen),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalReadRequests, ConsoleColor.DarkGreen));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total hits", ConsoleColor.DarkGreen),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalCacheHits, ConsoleColor.DarkGreen));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Total request time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequestTime, ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average request time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRequestTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache costs", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRequestTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average repository read time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryReadTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache read time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageReadCacheTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average deserialization time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average repository write time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryWriteTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average cache write time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageWriteCacheTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Average serialization time", ConsoleColor.Cyan),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageSerializationTime.TotalMicroseconds} мкс", ConsoleColor.Cyan));

        tableConsoleWriter.Write();
    }
}