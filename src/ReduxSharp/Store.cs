using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReduxSharp.Internal;

namespace ReduxSharp
{
    /// <summary>
    /// A store that holds the complete state tree of your application.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public sealed class Store<TState> : IStore<TState>, IObserverLinkedList<TState>
    {
        readonly IReducer<TState> reducer;
        readonly IMiddlewareFactory<TState>[] factories;
        readonly object stateLock = new object();
        readonly object buildLock = new object();
        readonly HashSet<Type> building = new HashSet<Type>();
        // Published dictionaries are immutable. A cache hit needs no build lock.
        volatile Dictionary<Type, Delegate> entries = new Dictionary<Type, Delegate>();
        ObserverNode<TState>? root;
        ObserverNode<TState>? last;

        internal Store(TState state, IReducer<TState> reducer, IMiddlewareFactory<TState>[] factories)
        {
            State = state;
            this.reducer = reducer;
            this.factories = factories;
        }

        /// <inheritdoc/>
        public TState State { get; private set; }

        /// <inheritdoc/>
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
                        lock (stateLock)
                        {
                            state = reduce(State, action);
                            State = state;
                        }
                        OnNext(state);
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

        /// <summary>
        /// Notifies the provider that an observer is to receive notifications.
        /// </summary>
        /// <param name="observer">
        /// The object that is to receive notifications.
        /// </param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications
        /// before the provider has finished sending them.
        /// </returns>
        public IDisposable Subscribe(IObserver<TState> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            var next = new ObserverNode<TState>(this, observer);
            if (root == null)
            {
                root = last = next;
            }
            else
            {
                last!.Next = next;
                next.Previous = last;
                last = next;
            }
            next.OnNext(State);
            return next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void OnNext(TState value)
        {
            var node = root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        void IObserverLinkedList<TState>.UnsubscribeNode(ObserverNode<TState> node)
        {
            if (node == root)
            {
                root = node.Next;
            }
            if (node == last)
            {
                last = node.Previous;
            }
            if (node.Previous != null)
            {
                node.Previous.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Previous = node.Previous;
            }
        }
    }
}
