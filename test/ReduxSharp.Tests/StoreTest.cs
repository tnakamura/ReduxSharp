using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace ReduxSharp.Tests;

/// <summary>Verifies store construction, dispatch, and notifications.</summary>
public class StoreTest
{
    /// <summary>Holds the application state.</summary>
    public class AppState
    {
        /// <summary>Gets or sets the counter value.</summary>
        public int Count
        {
            get;
            set;
        }
    }

    /// <summary>Increments the counter.</summary>
    public class IncrementAction
    {
    }

    /// <summary>Decrements the counter.</summary>
    public class DecrementAction
    {
    }

    /// <summary>Carries an exception to be thrown by the test reducer.</summary>
    public class RaiseExceptionAction
    {
        /// <summary>Gets the exception raised by the test reducer.</summary>
        public Exception Exception
        {
            get;
        }

        /// <summary>Creates an action carrying the exception to raise.</summary>
        public RaiseExceptionAction(Exception exception)
        {
            Exception = exception;
        }
    }

    /// <summary>Computes the next application state from dispatched actions.</summary>
    public class AppReducer : IReducer<AppState>
    {
        /// <inheritdoc/>
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
                case RaiseExceptionAction exceptionAction:
                    throw exceptionAction.Exception;
                default:
                    return state;
            }
        }
    }

    class DelegateObserver<T> : IObserver<T>
    {
        readonly Action<T> _onNext;

        /// <summary>Creates an observer that forwards state updates to the callback.</summary>
        public DelegateObserver(Action<T> onNext)
        {
            _onNext = onNext;
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            _onNext(value);
        }
    }

    /// <summary>Verifies that a store can be constructed.</summary>
    [Fact]
    public void BuildTest()
    {
        Store<AppState> store = new StoreBuilder<AppState>(default).Build(new AppReducer());
        Assert.NotNull(store);
    }

    /// <summary>Verifies that store construction rejects a null reducer.</summary>
    [Fact]
    public void BuildNullReducerTest()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            new StoreBuilder<AppState>(default).Build(null);
        });
    }

    /// <summary>Verifies that dispatch applies the reducer.</summary>
    [Fact]
    public void DispatchActionTest()
    {
        Store<AppState> store = new StoreBuilder<AppState>(new AppState()).Build(new AppReducer());
        store.Dispatch(new IncrementAction());
        Assert.Equal(1, store.State.Count);
    }

    /// <summary>Verifies that concurrent dispatches preserve all state updates.</summary>
    [Fact]
    public async Task DispatchThreadSafeTest()
    {
        Store<AppState> store = new StoreBuilder<AppState>(new AppState()).Build(new AppReducer());

        await Task.WhenAll(Enumerable.Range(0, 1000).Select(_ => Task.Run(() =>
        {
            store.Dispatch(new IncrementAction());
        })));

        Assert.Equal(1000, store.State.Count);
    }

    /// <summary>Records action execution in the test log.</summary>
    public class LoggingMiddleware<TState> : IMiddlewareFactory<TState>
    {
        /// <summary>Gets the recorded action execution messages.</summary>
        public List<string> Logs { get; } = new List<string>();

        /// <inheritdoc/>
        public Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next) => action =>
        {
            Logs.Add($"Executing {action.GetType().Name}");

            next(action);

            Logs.Add($"Executed {action.GetType().Name}");
        };
    }

    /// <summary>Records actions and their before and after states.</summary>
    public class TimeTravelMiddleware<TState> : IMiddlewareFactory<TState>
    {
        /// <summary>Gets the recorded state transitions.</summary>
        public List<History> Histories { get; } = new List<History>();

        /// <inheritdoc/>
        public Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next) => action =>
        {
            var history = new History
            {
                Action = JsonConvert.SerializeObject(action),
                BeforeState = JsonConvert.SerializeObject(store.State),
            };
            Histories.Add(history);

            next(action);

            history.AfterState = JsonConvert.SerializeObject(store.State);
        };

        /// <summary>Holds a serialized action and its before and after states.</summary>
        public class History
        {
            /// <summary>Gets or sets the serialized state before dispatch.</summary>
            public string BeforeState
            {
                get;
                set;
            }

            /// <summary>Gets or sets the serialized state after dispatch.</summary>
            public string AfterState
            {
                get;
                set;
            }

            /// <summary>Gets or sets the serialized action.</summary>
            public string Action
            {
                get;
                set;
            }
        }
    }

    /// <summary>Verifies that middleware surrounds reducer execution.</summary>
    [Fact]
    public void MiddlewareTest()
    {
        var middleware = new LoggingMiddleware<AppState>();
        Store<AppState> store = new StoreBuilder<AppState>(new AppState()).Use(middleware).Build(new AppReducer());

        store.Dispatch(new IncrementAction());

        Assert.Equal(1, store.State.Count);
        Assert.Equal(2, middleware.Logs.Count);
        Assert.Equal("Executing IncrementAction", middleware.Logs[0]);
        Assert.Equal("Executed IncrementAction", middleware.Logs[1]);
    }

    /// <summary>Verifies that subscription immediately publishes the current state.</summary>
    [Fact]
    public void SubscribeImmediatelyNotifiesObserver()
    {
        Store<AppState> store = new StoreBuilder<AppState>(new AppState()).Build(new AppReducer());

        AppState actualState = null;
        store.Subscribe(new DelegateObserver<AppState>(state =>
        {
            actualState = state;
        }));

        Assert.NotNull(actualState);
        Assert.Same(store.State, actualState);
    }

    /// <summary>Verifies that dispatch publishes the updated state.</summary>
    [Fact]
    public void DispatchNotifiesObserverOfUpdatedState()
    {
        Store<AppState> store = new StoreBuilder<AppState>(new AppState()).Build(new AppReducer());

        var actualStates = new List<AppState>();
        store.Subscribe(new DelegateObserver<AppState>(state =>
        {
            actualStates.Add(state);
        }));

        store.Dispatch(new IncrementAction());

        Assert.Equal(2, actualStates.Count);
        Assert.Equal(0, actualStates[0].Count);
        Assert.Equal(1, actualStates[1].Count);
    }

    /// <summary>Verifies that middleware records each state transition.</summary>
    [Fact]
    public void TimeTravelTest()
    {
        var middleware = new TimeTravelMiddleware<AppState>();
        Store<AppState> store = new StoreBuilder<AppState>(new AppState()).Use(middleware).Build(new AppReducer());

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
