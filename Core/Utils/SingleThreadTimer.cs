namespace Core.Utils;

public class SingleThreadTimer : IDisposable
{
    private readonly Func<Task> callback;
    private readonly PeriodicTimer timer;
    private bool isActive;

    public SingleThreadTimer(Func<Task> callback, TimeSpan interval)
    {
        this.callback = callback;
        timer = new PeriodicTimer(interval);
    }

    public async Task Start()
    {
        isActive = true;
        await Task.Factory.StartNew(async () =>
        {
            while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
            {
                if (!isActive) break;
                await callback();
            }
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Stop()
    {
        isActive = false;
    }

    public void Dispose()
    {
        Stop();
        timer.Dispose();
    }
}