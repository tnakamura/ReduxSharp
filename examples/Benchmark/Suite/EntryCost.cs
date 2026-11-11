using BenchmarkDotNet.Attributes;
using ReduxSharp;

namespace Benchmark.Suite;

/// <summary>Measures the first dispatch through a newly constructed store.</summary>
[Config(typeof(BenchmarkConfig))]
public class EntryCost
{
    const int BatchSize = 1024;
    readonly Store<AppState>[] _freshStores = new Store<AppState>[BatchSize];
    StoreBuilder<AppState> _builder;
    readonly AppReducer _reducer = new();

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
        _builder = new StoreBuilder<AppState>(new AppState());
        for (var i = 0; i < Depth; i++)
        {
            _builder.Use(new PassThrough());
        }
    }

    /// <summary>Creates stores whose action pipelines have not been built.</summary>
    [IterationSetup(Target = nameof(FirstDispatch))]
    public void PrepareColdEntries()
    {
        for (var i = 0; i < _freshStores.Length; i++)
        {
            _freshStores[i] = _builder.Build(_reducer);
        }
    }

    /// <summary>Dispatches the first value type action to each fresh store.</summary>
    [Benchmark(OperationsPerInvoke = BatchSize)]
    public void FirstDispatch()
    {
        for (var i = 0; i < _freshStores.Length; i++)
        {
            _freshStores[i].Dispatch(new StructIncrementAction());
        }
    }
}
