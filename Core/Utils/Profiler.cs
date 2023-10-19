using System.Diagnostics;

namespace Core.Utils;

public class Profiler : IDisposable
{
    private readonly Stopwatch sw = Stopwatch.StartNew();

    public void Restart() => sw.Restart();
    
    public TimeSpan ElapsedTime => sw.Elapsed;

    void IDisposable.Dispose()
    {
        sw.Stop();
    }
}
