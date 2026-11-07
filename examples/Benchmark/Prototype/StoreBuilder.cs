using System;
using System.Collections.Generic;
using System.Threading;
using ReduxSharp;

namespace Benchmark.Prototype
{
    public interface IMiddlewareFactory<TState>
    {
        Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next);
    }

    public interface IStoreBuilder<TState>
    {
        IStoreBuilder<TState> Use(IMiddlewareFactory<TState> factory);
        TypedStore<TState> Build(IReducer<TState> reducer);
    }

    public sealed class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        readonly TState initialState;
        readonly List<IMiddlewareFactory<TState>> factories = new();

        public StoreBuilder(TState initialState) => this.initialState = initialState;

        public IStoreBuilder<TState> Use(IMiddlewareFactory<TState> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            factories.Add(factory);
            return this;
        }

        public TypedStore<TState> Build(IReducer<TState> reducer)
        {
            ArgumentNullException.ThrowIfNull(reducer);
            return new TypedStore<TState>(initialState, reducer, factories.ToArray());
        }
    }

    public sealed class TypedStore<TState> : IStore<TState>
    {
        readonly IReducer<TState> reducer;
        readonly IMiddlewareFactory<TState>[] factories;
        readonly object stateLock = new();
        readonly object buildLock = new();
        readonly HashSet<Type> building = new();
        // Published dictionaries are immutable. A cache hit needs no build lock.
        volatile Dictionary<Type, Delegate> entries = new();
        Action<TState> observers;

        internal TypedStore(TState state, IReducer<TState> reducer, IMiddlewareFactory<TState>[] factories)
        {
            State = state;
            this.reducer = reducer;
            this.factories = factories;
        }

        public TState State { get; private set; }

        public void Dispatch<TAction>(TAction action)
        {
            var type = typeof(TAction);
            var dispatch = entries.TryGetValue(type, out var entry)
                ? (Action<TAction>)entry
                : BuildEntry<TAction>();
            dispatch(action);
        }

        Action<TAction> BuildEntry<TAction>()
        {
            var type = typeof(TAction);
            lock (buildLock)
            {
                if (entries.TryGetValue(type, out var entry)) return (Action<TAction>)entry;
                if (!building.Add(type)) throw new InvalidOperationException("Recursive pipeline construction.");
                try
                {
                    Func<TState, TAction, TState> reduce = reducer.Invoke<TAction>;
                    Action<TAction> next = action =>
                    {
                        TState state;
                        Action<TState> notify;
                        lock (stateLock)
                        {
                            State = state = reduce(State, action);
                            notify = observers;
                        }
                        notify?.Invoke(state);
                    };
                    for (var i = factories.Length - 1; i >= 0; i--)
                    {
                        next = factories[i].Build(this, next)
                            ?? throw new InvalidOperationException("Middleware returned a null dispatcher.");
                    }
                    entries = new Dictionary<Type, Delegate>(entries) { [type] = next };
                    return next;
                }
                finally
                {
                    building.Remove(type);
                }
            }
        }

        public IDisposable Subscribe(IObserver<TState> observer)
        {
            ArgumentNullException.ThrowIfNull(observer);
            Action<TState> notify = observer.OnNext;
            TState state;
            lock (stateLock)
            {
                observers += notify;
                state = State;
            }
            var subscription = new Subscription(() =>
            {
                lock (stateLock) observers -= notify;
            });
            try
            {
                notify(state);
                return subscription;
            }
            catch
            {
                subscription.Dispose();
                throw;
            }
        }

        sealed class Subscription : IDisposable
        {
            Action remove;
            public Subscription(Action remove) => this.remove = remove;
            public void Dispose() => Interlocked.Exchange(ref remove, null)?.Invoke();
        }
    }
}
