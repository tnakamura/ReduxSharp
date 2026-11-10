using System;
using ReduxSharp;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Benchmark.Suite
{
    internal sealed class PassThrough : IMiddlewareFactory<AppState>
    {
        public Action<TAction> Build<TAction>(ReduxSharp.IStore<AppState> store, Action<TAction> next)
            => action => next(action);
    }

    [Config(typeof(BenchmarkConfig))]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    public class Middleware
    {
        ReduxSharp.Store<AppState> sharp;
        Redux.Store<AppState> redux;
        readonly ClassIncrementAction cached = new();

        [Params(0, 1, 3, 8)]
        public int Depth { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var delegates = new Redux.Middleware<AppState>[Depth];
            var builder = new StoreBuilder<AppState>(new AppState());
            for (var i = 0; i < Depth; i++)
            {
                delegates[i] = _ => next => action => next(action);
                builder.Use(new PassThrough());
            }
            redux = new Redux.Store<AppState>(AppReducer.Reduce, new AppState(), delegates);
            sharp = builder.Build(new AppReducer());
        }

        [Benchmark(Baseline = true), BenchmarkCategory("Struct")]
        public Redux.IAction ReduxNetStruct() => redux.Dispatch(new StructIncrementAction());
        [Benchmark, BenchmarkCategory("Struct")]
        public void ReduxSharpStruct() => sharp.Dispatch(new StructIncrementAction());

        [Benchmark(Baseline = true), BenchmarkCategory("Class")]
        public Redux.IAction ReduxNetClass() => redux.Dispatch(new ClassIncrementAction());
        [Benchmark, BenchmarkCategory("Class")]
        public void ReduxSharpClass() => sharp.Dispatch(new ClassIncrementAction());

        [Benchmark(Baseline = true), BenchmarkCategory("CachedClass")]
        public Redux.IAction ReduxNetCached() => redux.Dispatch(cached);
        [Benchmark, BenchmarkCategory("CachedClass")]
        public void ReduxSharpCached() => sharp.Dispatch(cached);
    }
}
