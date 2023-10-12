namespace Core.Metric;

public class MetricsStorage
{
    private readonly List<Metrics> metricList = new();

    public void Add(Metrics metrics)
    {
        metricList.Add(metrics);
    }

    public IEnumerable<Metrics> GetAll() => metricList.AsReadOnly();

    public IEnumerable<Metrics> GetSlice(DateTime from, DateTime to)
    {
        return metricList
            .Where(x => x.Timestamp >= from && x.Timestamp <= to)
            .OrderBy(x => x.Timestamp);
    }
}
