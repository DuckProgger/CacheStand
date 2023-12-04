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
            new ConsoleTableConsoleWriter.Column("Параметр", 35),
            new ConsoleTableConsoleWriter.Column("Значение", 20));
        
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Эффективность кэша", percantageMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.CacheEfficiency:.##} %", percantageMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Частота попаданий", percantageMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.HitRate:.##} %", percantageMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Запросов в секунду", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.RequestsPerSecond, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Всего запросов", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequests, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Всего запросов на чтение", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalReadRequests, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Всего попаданий в кэш", numberMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalCacheHits, numberMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Общее время запросов", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell(metricsResult.TotalRequestTime, timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время запроса", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRequestTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Средние затраты на кэширование", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageCacheTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время чтения из хранилища", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryReadTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время чтения из кэша", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageReadCacheTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время на десериализацию", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageDeserializationTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время записи в хранилище", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageRepositoryWriteTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время записи в кэш", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageWriteCacheTime.TotalMicroseconds} мкс", timeMetricsColor));
        tableConsoleWriter.AppendRow(
            new ConsoleTableConsoleWriter.Cell("Среднее время сериализации", timeMetricsColor),
            new ConsoleTableConsoleWriter.Cell($"{metricsResult.AverageSerializationTime.TotalMicroseconds} мкс", timeMetricsColor));

        tableConsoleWriter.Write();
    }
}