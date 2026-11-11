using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ReduxSharp;

namespace Benchmark.Suite;

internal sealed class PassThrough : IMiddlewareFactory<AppState>
{
    /// <inheritdoc/>
    public Action<TAction> Build<TAction>(ReduxSharp.IStore<AppState> store, Action<TAction> next)
        => action => next(action);
}

/// <summary>Compares dispatch costs at different middleware depths.</summary>
[Config(typeof(BenchmarkConfig))]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
[CategoriesColumn]
public class Middleware
{
    ReduxSharp.Store<AppState> _reduxSharpStore;
    Redux.Store<AppState> _reduxDotNetStore;
    readonly ClassIncrementAction _cachedAction = new();

    /// <summary>Gets or sets the number of middleware stages.</summary>
    [Params(0, 1, 3, 8)]
    public int Depth
    {
        get;
        set;
    }

    /// <summary>Creates the stores used by the benchmark.</summary>
    [GlobalSetup]
    public void Setup()
    {
        Redux.Middleware<AppState>[] delegates = new Redux.Middleware<AppState>[Depth];
        var builder = new StoreBuilder<AppState>(new AppState());
        for (var i = 0; i < Depth; i++)
        {
            delegates[i] = _ => next => action => next(action);
            builder.Use(new PassThrough());
        }
        _reduxDotNetStore = new Redux.Store<AppState>(AppReducer.Reduce, new AppState(), delegates);
        _reduxSharpStore = builder.Build(new AppReducer());
    }

    /// <summary>Dispatches a value type action through Redux.NET.</summary>
    [Benchmark(Baseline = true), BenchmarkCategory("Struct")]
    public Redux.IAction ReduxNetStruct() => _reduxDotNetStore.Dispatch(new StructIncrementAction());

    /// <summary>Dispatches a value type action through ReduxSharp.</summary>
    [Benchmark, BenchmarkCategory("Struct")]
    public void ReduxSharpStruct() => _reduxSharpStore.Dispatch(new StructIncrementAction());

    /// <summary>Dispatches a new reference type action through Redux.NET.</summary>
    [Benchmark(Baseline = true), BenchmarkCategory("Class")]
    public Redux.IAction ReduxNetClass() => _reduxDotNetStore.Dispatch(new ClassIncrementAction());

    /// <summary>Dispatches a new reference type action through ReduxSharp.</summary>
    [Benchmark, BenchmarkCategory("Class")]
    public void ReduxSharpClass() => _reduxSharpStore.Dispatch(new ClassIncrementAction());

    /// <summary>Dispatches a cached reference type action through Redux.NET.</summary>
    [Benchmark(Baseline = true), BenchmarkCategory("CachedClass")]
    public Redux.IAction ReduxNetCached() => _reduxDotNetStore.Dispatch(_cachedAction);

    /// <summary>Dispatches a cached reference type action through ReduxSharp.</summary>
    [Benchmark, BenchmarkCategory("CachedClass")]
    public void ReduxSharpCached() => _reduxSharpStore.Dispatch(_cachedAction);
}
