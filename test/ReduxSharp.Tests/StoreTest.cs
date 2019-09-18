using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace ReduxSharp.Tests
{
    public class StoreTest
    {
        public class AppState
        {
            public int Count { get; set; }
        }

        public class IncrementAction { }

        public class DecrementAction { }

        public class RaiseExceptionAction
        {
            public Exception Exception { get; }

            public RaiseExceptionAction(Exception exception)
            {
                Exception = exception;
            }
        }

        [Fact]
        public void ConstructorTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), default);
            Assert.NotNull(store);
        }

        [Fact]
        public void ConstructorNullReducerTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Store<AppState>(null as IReducer<AppState>, default);
            });
        }

        [Fact]
        public void DispatchActionTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());
            store.Dispatch(new IncrementAction());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public async Task DispatchThreadSafeTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());

            await Task.WhenAll(Enumerable.Range(0, 1000).Select(_ => Task.Run(() =>
            {
                store.Dispatch(new IncrementAction());
            })));

            Assert.Equal(1000, store.State.Count);
        }

        public class AsyncAppReducer : IReducer<AppState>
        {
            public AppState Invoke<TAction>(AppState state, TAction action)
            {
                state = state ?? new AppState
                {
                    Count = 0,
                };

                switch (action)
                {
                    case IncrementAction _:
                        return new AppState
                        {
                            Count = state.Count + 1,
                        };
                    case DecrementAction _:
                        return new AppState
                        {
                            Count = state.Count - 1,
                        };
                    case RaiseExceptionAction e:
                        throw e.Exception;
                    default:
                        return state;
                }
            }
        }

        public class LoggingMiddleware<TState> : IMiddleware<TState>
        {
            public List<string> Logs { get; } = new List<string>();

            public void Invoke<TAction>(
                IStore<TState> store,
                IDispatcher next,
                TAction action)
            {
                Logs.Add($"Executing {action.GetType().Name}");

                next.Invoke(action);

                Logs.Add($"Executed {action.GetType().Name}");
            }
        }

        public class TimeTravelMiddleweare<TState> : IMiddleware<TState>
        {
            public List<History> Histories { get; } = new List<History>();

            public void Invoke<TAction>(
                IStore<TState> store,
                IDispatcher next,
                TAction action)
            {
                var history = new History();
                history.Action = JsonConvert.SerializeObject(action);
                history.BeforeState = JsonConvert.SerializeObject(store.State);
                Histories.Add(history);

                next.Invoke(action);

                history.AfterState = JsonConvert.SerializeObject(store.State);
            }

            public class History
            {
                public string BeforeState { get; set; }

                public string AfterState { get; set; }

                public string Action { get; set; }
            }
        }

        [Fact]
        public void MiddlewareTest()
        {
            var middleware = new LoggingMiddleware<AppState>();
            var store = new Store<AppState>(
                new AsyncAppReducer(),
                new AppState(),
                middleware);

            store.Dispatch(new IncrementAction());

            Assert.Equal(1, store.State.Count);
            Assert.Equal(2, middleware.Logs.Count);
            Assert.Equal("Executing IncrementAction", middleware.Logs[0]);
            Assert.Equal("Executed IncrementAction", middleware.Logs[1]);
        }

        [Fact]
        public void TimeTravelTest()
        {
            var middleware = new TimeTravelMiddleweare<AppState>();
            var store = new Store<AppState>(
                new AsyncAppReducer(),
                new AppState(),
                middleware);

            store.Dispatch(new IncrementAction());
            store.Dispatch(new IncrementAction());
            store.Dispatch(new DecrementAction());
            store.Dispatch(new IncrementAction());

            Assert.Equal(2, store.State.Count);
            Assert.Equal(4, middleware.Histories.Count);

            Assert.NotNull(JsonConvert.DeserializeObject<IncrementAction>(middleware.Histories[0].Action));
            Assert.Equal(0, JsonConvert.DeserializeObject<AppState>(middleware.Histories[0].BeforeState).Count);
            Assert.Equal(1, JsonConvert.DeserializeObject<AppState>(middleware.Histories[0].AfterState).Count);

            Assert.NotNull(JsonConvert.DeserializeObject<IncrementAction>(middleware.Histories[1].Action));
            Assert.Equal(1, JsonConvert.DeserializeObject<AppState>(middleware.Histories[1].BeforeState).Count);
            Assert.Equal(2, JsonConvert.DeserializeObject<AppState>(middleware.Histories[1].AfterState).Count);

            Assert.NotNull(JsonConvert.DeserializeObject<DecrementAction>(middleware.Histories[2].Action));
            Assert.Equal(2, JsonConvert.DeserializeObject<AppState>(middleware.Histories[2].BeforeState).Count);
            Assert.Equal(1, JsonConvert.DeserializeObject<AppState>(middleware.Histories[2].AfterState).Count);

            Assert.NotNull(JsonConvert.DeserializeObject<IncrementAction>(middleware.Histories[3].Action));
            Assert.Equal(1, JsonConvert.DeserializeObject<AppState>(middleware.Histories[3].BeforeState).Count);
            Assert.Equal(2, JsonConvert.DeserializeObject<AppState>(middleware.Histories[3].AfterState).Count);
        }
    }
}
