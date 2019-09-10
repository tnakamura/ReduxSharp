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
        public async Task DispatchActionTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());
            await store.Dispatch(new IncrementAction());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public async Task DispatchThreadSafeTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());

            await Task.WhenAll(Enumerable.Range(0, 1000).Select(async _ =>
            {
                await store.Dispatch(new IncrementAction());
            }));

            Assert.Equal(1000, store.State.Count);
        }

        [Fact]
        public async Task DispatchHandleExceptionTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());

            Exception actual = null;
            var observer = new ActionObserver<AppState>()
            {
                Error = (error) =>
                {
                    actual = error;
                }
            };
            store.Subscribe(observer);

            await store.Dispatch(
                new RaiseExceptionAction(
                    new NotSupportedException()));

            Assert.NotNull(actual);
            Assert.IsType<NotSupportedException>(actual);
        }

        public class AsyncAppReducer : IReducer<AppState>
        {
            public async ValueTask<AppState> Invoke<TAction>(AppState state, TAction action)
            {
                state = state ?? new AppState
                {
                    Count = 0,
                };

                switch (action)
                {
                    case IncrementAction _:
                        return await Task.Run(() =>
                        {
                            return new AppState
                            {
                                Count = state.Count + 1,
                            };
                        });
                    case DecrementAction _:
                        return await Task.Run(() =>
                        {
                            return new AppState
                            {
                                Count = state.Count - 1,
                            };
                        });
                    case RaiseExceptionAction e:
                        throw e.Exception;
                    default:
                        return state;
                }
            }
        }

        public class AsyncLogMiddleware<TState> : IMiddleware<TState>
        {
            public List<string> Logs { get; } = new List<string>();

            public async ValueTask Invoke<TAction>(
                IStore<TState> store,
                IDispatcher next,
                TAction action)
            {
                Logs.Add($"Executing {action.GetType().Name}");

                await next.Invoke(action).ConfigureAwait(false);

                Logs.Add($"Executed {action.GetType().Name}");
            }
        }

        public class TimeTravelMiddleweare<TState> : IMiddleware<TState>
        {
            public List<History> Histories { get; } = new List<History>();

            public async ValueTask Invoke<TAction>(
                IStore<TState> store,
                IDispatcher next,
                TAction action)
            {
                var history = new History();
                history.Action = JsonConvert.SerializeObject(action);
                history.BeforeState = JsonConvert.SerializeObject(store.State);
                Histories.Add(history);

                await next.Invoke(action);

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
        public async Task DispatchAsyncActionTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());
            await store.Dispatch(new IncrementAction());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public async Task MiddlewareTest()
        {
            var middleware = new AsyncLogMiddleware<AppState>();
            var store = new Store<AppState>(
                new AsyncAppReducer(),
                new AppState(),
                middleware);

            await store.Dispatch(new IncrementAction());

            Assert.Equal(1, store.State.Count);
            Assert.Equal(2, middleware.Logs.Count);
            Assert.Equal("Executing IncrementAction", middleware.Logs[0]);
            Assert.Equal("Executed IncrementAction", middleware.Logs[1]);
        }

        [Fact]
        public async Task TimeTravelTest()
        {
            var middleware = new TimeTravelMiddleweare<AppState>();
            var store = new Store<AppState>(
                new AsyncAppReducer(),
                new AppState(),
                middleware);

            await store.Dispatch(new IncrementAction());
            await store.Dispatch(new IncrementAction());
            await store.Dispatch(new DecrementAction());
            await store.Dispatch(new IncrementAction());

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

        [Fact]
        public async Task ThunkTest()
        {
            var middleware = new ThunkMiddleware<AppState>();
            var store = new Store<AppState>(
                new AsyncAppReducer(),
                new AppState(),
                middleware);

            await store.Dispatch(new Func<IDispatcher, ValueTask>(async d =>
            {
                await d.Invoke(new IncrementAction());
                await d.Invoke(new IncrementAction());
                await d.Invoke(new IncrementAction());
            }));

            Assert.Equal(3, store.State.Count);
        }
    }
}
