﻿using BenchmarkDotNet.Attributes;

namespace Benchmark.Suite
{
    [Config(typeof(BenchmarkConfig))]
    public class Middleware
    {
        ReduxSharp.Store<AppState> reduxSharpStore;

        Redux.Store<AppState> reduxDotNetStore;

        [GlobalSetup]
        public void Setup()
        {
            reduxSharpStore = new ReduxSharp.Store<AppState>(
                new AppReducer(),
                new AppState(),
                new EmptyMiddleware<AppState>());

            reduxDotNetStore = new Redux.Store<AppState>(
                AppReducer.Reduce,
                new AppState(),
                EmptyMiddleware<AppState>.ApplyMiddleware);
        }

        [Benchmark]
        public Redux.IAction ReduxDotNet_struct()
        {
            return reduxDotNetStore.Dispatch(new StructIncrementAction());
        }

        [Benchmark]
        public void ReduxSharp_struct()
        {
            reduxSharpStore.Dispatch(new StructIncrementAction());
        }

        [Benchmark]
        public Redux.IAction ReduxDotNet_class()
        {
            return reduxDotNetStore.Dispatch(new ClassIncrementAction());
        }

        [Benchmark]
        public void ReduxSharp_class()
        {
            reduxSharpStore.Dispatch(new ClassIncrementAction());
        }
    }
}
