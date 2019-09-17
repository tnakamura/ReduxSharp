using System;
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

        public AppState Invoke<TAction>(AppState state, in TAction action) => state;
    }

    public sealed class EmptyMiddleware<TState> : ReduxSharp.IMiddleware<TState>
    {
        public void Invoke<TAction>(
            ReduxSharp.IStore<TState> store,
            ReduxSharp.IDispatcher next,
            in TAction action)
        {
            next.Invoke(action);
        }

        public static Func<Redux.Dispatcher, Redux.Dispatcher> ApplyMiddleware(
            Redux.IStore<TState> store)
        {
            return next => action => next(action);
        }
    }
}
