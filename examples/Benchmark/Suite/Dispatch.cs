using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;

namespace Benchmark.Suite
{
    [Config(typeof(BenchmarkConfig))]
    public class Dispatch
    {
        ReduxSharp.Store<AppState> reduxSharpStore;

        Redux.Store<AppState> reduxDotNetStore;

        [GlobalSetup]
        public void Setup()
        {
            reduxSharpStore = new ReduxSharp.Store<AppState>(
                new AppReducer(),
                new AppState());

            reduxDotNetStore = new Redux.Store<AppState>(
                AppReducer.Reduce,
                new AppState());
        }

        [Benchmark(Baseline = true)]
        public object ReduxDotNet()
        {
            return reduxDotNetStore.Dispatch(new IncrementAction());
        }

        [Benchmark]
        public ValueTask ReduxSharp()
        {
            return reduxSharpStore.Dispatch(new IncrementAction());
        }
    }
}
