namespace System;

/// <summary>
/// Contains extension methods for profiling code execution and measuring performance.
/// </summary>
public static class ProfilingExtensions
{
    /// <summary>
    /// Measures the execution time of an action.
    /// </summary>
    /// <param name="action">The action to measure.</param>
    /// <returns>The time taken to execute the action.</returns>
    public static TimeSpan MeasureExecutionTime(this Action action)
    {
        var stopwatch = Stopwatch.StartNew();

        action();

        stopwatch.Stop();

        return stopwatch.Elapsed;
    }

    /// <summary>
    /// Measures the execution time of a function and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to measure.</param>
    /// <param name="result">The result returned by the function.</param>
    /// <returns>The time taken to execute the function.</returns>
    public static TimeSpan MeasureExecutionTime<T>(this Func<T> func, out T result)
    {
        var stopwatch = Stopwatch.StartNew();

        result = func();

        stopwatch.Stop();

        return stopwatch.Elapsed;
    }
}