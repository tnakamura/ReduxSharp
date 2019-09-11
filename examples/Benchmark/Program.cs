using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ReduxDotNetVsReduxSharp>();
        }
    }

    public class ReduxDotNetVsReduxSharp
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

    public class AppState
    {
        public int Count { get; set; }
    }

    public struct IncrementAction : Redux.IAction
    {
    }

    public class AppReducer : ReduxSharp.IReducer<AppState>
    {
        public static AppState Reduce(AppState state, object action)
        {
            switch (action)
            {
                case IncrementAction _:
                    return new AppState
                    {
                        Count = state.Count + 1,
                    };
                default:
                    return state;
            }
        }

        public ValueTask<AppState> Invoke<TAction>(AppState state, TAction action)
        {
            switch (action)
            {
                case IncrementAction _:
                    return new ValueTask<AppState>(new AppState
                    {
                        Count = state.Count + 1,
                    });
                default:
                    return new ValueTask<AppState>(state);
            }
        }
    }
}
