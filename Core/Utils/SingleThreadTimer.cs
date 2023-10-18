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

    public async Task Start(CancellationToken cancellationToken = default)
    {
        isActive = true;

        await Task.Run(async () =>
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!isActive) break;

                await callback();
            }

        }, CancellationToken.None);
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