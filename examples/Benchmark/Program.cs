using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
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
