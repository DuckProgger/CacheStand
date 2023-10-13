using System.Diagnostics;
using Core.Data;

namespace Core.Utils;

public class Stopwatcher
{
    public TimeSpan ElapsedTime { get; private set; }

    public async Task<TResult> DoAsyncOperation<TInput1, TInput2, TResult>(Func<TInput1, TInput2, Task<TResult>> func,
        TInput1 input1, TInput2 input2)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await func(input1, input2);
        stopwatch.Stop();
        ElapsedTime = stopwatch.Elapsed;
        return result;
    }
    
    public async Task<TResult> DoAsyncOperation<TInput, TResult>(Func<TInput, Task<TResult>> func, TInput input)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = await func(input);
        stopwatch.Stop();
        ElapsedTime = stopwatch.Elapsed;
        return result;
    }
    
    public TResult DoOperation<TInput, TResult>(Func<TInput, TResult> func, TInput input)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var result = func(input);
        stopwatch.Stop();
        ElapsedTime = stopwatch.Elapsed;
        return result;
    }
}