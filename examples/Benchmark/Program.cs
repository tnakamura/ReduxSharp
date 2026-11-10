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


    public sealed class AppState
    {
        public int Count { get; set; }
    }

    public struct StructIncrementAction : Redux.IAction { }

    public sealed class ClassIncrementAction : Redux.IAction { }

    public sealed class AppReducer : ReduxSharp.IReducer<AppState>
    {
        public static AppState Reduce(AppState state, Redux.IAction action) => state;

        public AppState Invoke<TAction>(AppState state, TAction action) => state;
    }
}
