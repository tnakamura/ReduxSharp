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

            public class IncrementAction : IAction { }

            public class DecrementAction : IAction { }
        }

        public static class AppActionCreators
        {
            public static ActionCreator<AppState> Increment()
            {
                return (state, store) =>
                {
                    return new AppState.IncrementAction();
                };
            }

            public static AsyncActionCreator<AppState> IncrementTwice()
            {
                return async (state, store, callback) =>
                {
                    callback(Increment());
                    await Task.Delay(10);
                    callback(Increment());
                };
            }
        }

        public static class AppReducer
        {
            public static AppState Invoke(AppState state, IAction action)
            {
                state = state ?? new AppState() { Count = 0 };

                if (action is AppState.IncrementAction)
                {
                    state.Count += 1;
                }

                return state;
            }
        }

        [Fact]
        public void ConstructorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            Assert.NotNull(store);
        }

        [Fact]
        public void ConstructorNullReducerTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Store<AppState>(null as Reducer<AppState>);
            });
        }

        [Fact]
        public void DispatchNullActiontest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.Dispatch(default(IAction));
            });
        }

        [Fact]
        public void DispatchActionTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            store.Dispatch(new AppState.IncrementAction());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public void DispatchActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            store.Dispatch(AppActionCreators.Increment());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public async Task DispatchAsyncActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            await store.Dispatch(AppActionCreators.IncrementTwice());
            Assert.Equal(2, store.State.Count);
        }

        [Fact]
        public void DispatchNullActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            ActionCreator<AppState> actionCreator = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.Dispatch(actionCreator);
            });
        }

        [Fact]
        public async Task DispatchNullAsyncActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            AsyncActionCreator<AppState> asyncActionCreator = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await store.Dispatch(asyncActionCreator);
            });
        }

        [Fact]
        public async Task DispatchThreadSafeTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke, new AppState());

            await Task.WhenAll(Enumerable.Range(0, 1000).Select(_ => Task.Run(() =>
              {
                  store.Dispatch(new AppState.IncrementAction());
              })));

            Assert.Equal(1000, store.State.Count);
        }

        [Fact]
        public void DispatchHandleExceptionTest()
        {
            AppState Reducer(AppState state, IAction action)
            {
                if (action is StandardAction std && std.Type == "test")
                {
                    throw new NotSupportedException();
                }
                return state ?? new AppState();
            }

            var store = new Store<AppState>(Reducer, new AppState());

            Exception actual = null;
            var observer = new ActionObserver<AppState>()
            {
                Error = (error) =>
                {
                    actual = error;
                }
            };
            store.Subscribe(observer);

            store.Dispatch(new StandardAction("test"));

            Assert.NotNull(actual);
            Assert.IsType<NotSupportedException>(actual);
        }

        [Fact]
        public void DispatchActionCreatorHandleExceptionTest()
        {
            AppState Reducer(AppState state, IAction action)
            {
                if (action is StandardAction std && std.Type == "test")
                {
                    throw new NotSupportedException();
                }
                return state ?? new AppState();
            }

            var store = new Store<AppState>(Reducer, new AppState());

            Exception actual = null;
            var observer = new ActionObserver<AppState>()
            {
                Error = (error) =>
                {
                    actual = error;
                }
            };
            store.Subscribe(observer);

            store.Dispatch((_, __) => new StandardAction("test"));

            Assert.NotNull(actual);
            Assert.IsType<NotSupportedException>(actual);
        }

        [Fact]
        public void DispatchActionCreatorThatThrowsExcepitionHandleExceptionTest()
        {
            AppState Reducer(AppState state, IAction action)
            {
                return state ?? new AppState();
            }

            var store = new Store<AppState>(Reducer, new AppState());

            Exception actual = null;
            var observer = new ActionObserver<AppState>()
            {
                Error = (error) =>
                {
                    actual = error;
                }
            };
            store.Subscribe(observer);

            store.Dispatch((_, __) =>
            {
                throw new NotSupportedException();
            });

            Assert.NotNull(actual);
            Assert.IsType<NotSupportedException>(actual);
        }

        [Fact]
        public async Task DispatchAsyncActionCreatorHandleExceptionTest()
        {
            AppState Reducer(AppState state, IAction action)
            {
                if (action is StandardAction std && std.Type == "test")
                {
                    throw new NotSupportedException();
                }
                return state ?? new AppState();
            }

            var asyncActionCreator = new AsyncActionCreator<AppState>(async (_, __, callback) =>
              {
                  await Task.FromResult(0);
                  callback((x, y) => new StandardAction("test"));
              });

            var store = new Store<AppState>(Reducer, new AppState());

            Exception actual = null;
            var observer = new ActionObserver<AppState>()
            {
                Error = (error) =>
                {
                    actual = error;
                }
            };
            store.Subscribe(observer);

            await store.Dispatch(asyncActionCreator);

            Assert.NotNull(actual);
            Assert.IsType<NotSupportedException>(actual);
        }

        [Fact]
        public async Task DispatchAsyncActionCreatorThatThrowsExcepitionHandleExceptionTest()
        {
            AppState Reducer(AppState state, IAction action)
            {
                return state ?? new AppState();
            }

            var asyncActionCreator = new AsyncActionCreator<AppState>(async (_, __, callback) =>
            {
                await Task.Yield();
                throw new NotSupportedException();
            });

            var store = new Store<AppState>(Reducer, new AppState());

            Exception actual = null;
            var observer = new ActionObserver<AppState>()
            {
                Error = (error) =>
                {
                    actual = error;
                }
            };
            store.Subscribe(observer);

            await store.Dispatch(asyncActionCreator);

            Assert.NotNull(actual);
            Assert.IsType<NotSupportedException>(actual);
        }

        public class AsyncAppReducer : IReducer<AppState>
        {
            public async Task<AppState> InvokeAsync<TAction>(AppState state, TAction action)
            {
                state = state ?? new AppState
                {
                    Count = 0,
                };

                switch (action)
                {
                    case AppState.IncrementAction _:
                        return await Task.Run(() =>
                        {
                            return new AppState
                            {
                                Count = state.Count + 1,
                            };
                        });
                    case AppState.DecrementAction _:
                        return await Task.Run(() =>
                        {
                            return new AppState
                            {
                                Count = state.Count - 1,
                            };
                        });
                    default:
                        return state;
                }
            }
        }

        public class AsyncLogMiddleware<TState> : IMiddleware<TState>
        {
            public List<string> Logs { get; } = new List<string>();

            public async Task InvokeAsync<TAction>(
                IStore<TState> store,
                Func<TAction, Task> next,
                TAction action)
            {
                Logs.Add($"Executing {action.GetType().Name}");

                await next(action).ConfigureAwait(false);

                Logs.Add($"Executed {action.GetType().Name}");
            }
        }

        public class TimeTravelMiddleweare<TState> : IMiddleware<TState>
        {
            public List<History> Histories { get; } = new List<History>();

            public async Task InvokeAsync<TAction>(IStore<TState> store, Func<TAction, Task> next, TAction action)
            {
                var history = new History();
                history.Action = JsonConvert.SerializeObject(action);
                history.State = JsonConvert.SerializeObject(store.State);
                Histories.Add(history);

                await next(action);
            }

            public class History
            {
                public string State { get; set; }

                public string Action { get; set; }
            }
        }

        [Fact]
        public async Task DispatchAsyncActionTest()
        {
            var store = new Store<AppState>(new AsyncAppReducer(), new AppState());
            await store.DispatchAsync(new AppState.IncrementAction());
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

            await store.DispatchAsync(new AppState.IncrementAction());

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

            await store.DispatchAsync(new AppState.IncrementAction());
            await store.DispatchAsync(new AppState.IncrementAction());
            await store.DispatchAsync(new AppState.DecrementAction());
            await store.DispatchAsync(new AppState.IncrementAction());

            Assert.Equal(2, store.State.Count);
            Assert.Equal(4, middleware.Histories.Count);

            Assert.NotNull(JsonConvert.DeserializeObject<AppState.IncrementAction>(middleware.Histories[0].Action));
            Assert.Equal(0, JsonConvert.DeserializeObject<AppState>(middleware.Histories[0].State).Count);

            Assert.NotNull(JsonConvert.DeserializeObject<AppState.IncrementAction>(middleware.Histories[1].Action));
            Assert.Equal(1, JsonConvert.DeserializeObject<AppState>(middleware.Histories[1].State).Count);

            Assert.NotNull(JsonConvert.DeserializeObject<AppState.DecrementAction>(middleware.Histories[2].Action));
            Assert.Equal(2, JsonConvert.DeserializeObject<AppState>(middleware.Histories[2].State).Count);

            Assert.NotNull(JsonConvert.DeserializeObject<AppState.IncrementAction>(middleware.Histories[3].Action));
            Assert.Equal(1, JsonConvert.DeserializeObject<AppState>(middleware.Histories[3].State).Count);
        }
    }
}
