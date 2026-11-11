using BenchmarkDotNet.Running;

namespace Benchmark;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}


/// <summary>Holds the application state.</summary>
public sealed class AppState
{
    /// <summary>Gets or sets the counter value.</summary>
    public int Count
    {
        get;
        set;
    }
}

/// <summary>Provides a value type action for dispatch benchmarks.</summary>
public struct StructIncrementAction : Redux.IAction
{
}

/// <summary>Provides a reference type action for dispatch benchmarks.</summary>
public sealed class ClassIncrementAction : Redux.IAction
{
}

/// <summary>Computes the next application state from dispatched actions.</summary>
public sealed class AppReducer : ReduxSharp.IReducer<AppState>
{
    /// <summary>Returns the state unchanged for the Redux.NET benchmark.</summary>
    public static AppState Reduce(AppState state, Redux.IAction action) => state;

    /// <inheritdoc/>
    public AppState Invoke<TAction>(AppState state, TAction action) => state;
}
