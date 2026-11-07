using System;
using Benchmark.Prototype;
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
        TypedStore<AppState> typed;
        readonly ClassIncrementAction cached = new();

        [Params(0, 1, 3, 8)]
        public int Depth { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            var middlewares = new ReduxSharp.IMiddleware<AppState>[Depth];
            var delegates = new Redux.Middleware<AppState>[Depth];
            var builder = new StoreBuilder<AppState>(new AppState());
            for (var i = 0; i < Depth; i++)
            {
                middlewares[i] = new EmptyMiddleware<AppState>();
                delegates[i] = EmptyMiddleware<AppState>.ApplyMiddleware;
                builder.Use(new PassThrough());
            }
            sharp = new ReduxSharp.Store<AppState>(new AppReducer(), new AppState(), middlewares);
            redux = new Redux.Store<AppState>(AppReducer.Reduce, new AppState(), delegates);
            typed = builder.Build(new AppReducer());
        }

        [Benchmark(Baseline = true), BenchmarkCategory("Struct")]
        public Redux.IAction ReduxNetStruct() => redux.Dispatch(new StructIncrementAction());
        [Benchmark, BenchmarkCategory("Struct")]
        public void ReduxSharpStruct() => sharp.Dispatch(new StructIncrementAction());
        [Benchmark, BenchmarkCategory("Struct")]
        public void TypedStruct() => typed.Dispatch(new StructIncrementAction());

        [Benchmark(Baseline = true), BenchmarkCategory("Class")]
        public Redux.IAction ReduxNetClass() => redux.Dispatch(new ClassIncrementAction());
        [Benchmark, BenchmarkCategory("Class")]
        public void ReduxSharpClass() => sharp.Dispatch(new ClassIncrementAction());
        [Benchmark, BenchmarkCategory("Class")]
        public void TypedClass() => typed.Dispatch(new ClassIncrementAction());

        [Benchmark(Baseline = true), BenchmarkCategory("CachedClass")]
        public Redux.IAction ReduxNetCached() => redux.Dispatch(cached);
        [Benchmark, BenchmarkCategory("CachedClass")]
        public void ReduxSharpCached() => sharp.Dispatch(cached);
        [Benchmark, BenchmarkCategory("CachedClass")]
        public void TypedCached() => typed.Dispatch(cached);
    }
}
