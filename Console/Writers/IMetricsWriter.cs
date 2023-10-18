using Core.Metric;

namespace Console.Writers;

public interface IMetricsWriter
{
    public void Write(MetricsResult data);
}