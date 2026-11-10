using ReduxSharp;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Suite
{
    [Config(typeof(BenchmarkConfig))]
    public class EntryCost
    {
        const int BatchSize = 1024;
        readonly Store<AppState>[] fresh = new Store<AppState>[BatchSize];
        StoreBuilder<AppState> builder;
        readonly AppReducer reducer = new();

        [Params(0, 1, 3, 8)]
        public int Depth { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            builder = new StoreBuilder<AppState>(new AppState());
            for (var i = 0; i < Depth; i++) builder.Use(new PassThrough());
        }

        [IterationSetup(Target = nameof(FirstDispatch))]
        public void PrepareColdEntries()
        {
            for (var i = 0; i < fresh.Length; i++) fresh[i] = builder.Build(reducer);
        }

        [Benchmark(OperationsPerInvoke = BatchSize)]
        public void FirstDispatch()
        {
            for (var i = 0; i < fresh.Length; i++) fresh[i].Dispatch(new StructIncrementAction());
        }
    }
}
