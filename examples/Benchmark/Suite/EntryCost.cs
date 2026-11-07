using System;
using Benchmark.Prototype;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Suite
{
    [Config(typeof(BenchmarkConfig))]
    public class EntryCost
    {
        const int BatchSize = 1024;
        readonly TypedStore<AppState>[] fresh = new TypedStore<AppState>[BatchSize];
        StoreBuilder<AppState> builder;
        TypedStore<AppState> warm;
        readonly AppReducer reducer = new();

        [Params(0, 1, 3, 8)]
        public int Depth { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            builder = new StoreBuilder<AppState>(new AppState());
            for (var i = 0; i < Depth; i++) builder.Use(new PassThrough());
            warm = builder.Run(reducer);
            warm.For<StructIncrementAction>();
        }

        [Benchmark]
        public void LookupAndDispatch() => warm.For<StructIncrementAction>()(new StructIncrementAction());

        [IterationSetup(Target = nameof(FirstFor))]
        public void PrepareColdEntries()
        {
            for (var i = 0; i < fresh.Length; i++) fresh[i] = builder.Run(reducer);
        }

        [Benchmark(OperationsPerInvoke = BatchSize)]
        public Action<StructIncrementAction> FirstFor()
        {
            Action<StructIncrementAction> entry = null;
            for (var i = 0; i < fresh.Length; i++) entry = fresh[i].For<StructIncrementAction>();
            return entry;
        }
    }
}
