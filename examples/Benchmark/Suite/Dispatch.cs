using BenchmarkDotNet.Attributes;

namespace Benchmark.Suite;

/// <summary>Measures dispatch costs for value and reference type actions.</summary>
[Config(typeof(BenchmarkConfig))]
public class Dispatch
{
    ReduxSharp.Store<AppState> _reduxSharpStore;

    Redux.Store<AppState> _reduxDotNetStore;

    /// <summary>Creates the stores used by the benchmark.</summary>
    [GlobalSetup]
    public void Setup()
    {
        _reduxSharpStore = new ReduxSharp.StoreBuilder<AppState>(new AppState()).Build(new AppReducer());

        _reduxDotNetStore = new Redux.Store<AppState>(
            AppReducer.Reduce,
            new AppState());
    }

    /// <summary>Dispatches a value type action through Redux.NET.</summary>
    [Benchmark]
    public Redux.IAction ReduxDotNetStruct()
    {
        return _reduxDotNetStore.Dispatch(new StructIncrementAction());
    }

    /// <summary>Dispatches a value type action through ReduxSharp.</summary>
    [Benchmark]
    public void ReduxSharpStruct()
    {
        _reduxSharpStore.Dispatch(new StructIncrementAction());
    }

    /// <summary>Dispatches a new reference type action through Redux.NET.</summary>
    [Benchmark]
    public Redux.IAction ReduxDotNetClass()
    {
        return _reduxDotNetStore.Dispatch(new ClassIncrementAction());
    }

    /// <summary>Dispatches a new reference type action through ReduxSharp.</summary>
    [Benchmark]
    public void ReduxSharpClass()
    {
        _reduxSharpStore.Dispatch(new ClassIncrementAction());
    }
}
