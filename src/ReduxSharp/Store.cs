using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ReduxSharp.Internal;

namespace ReduxSharp
{
    /// <summary>
    /// A store that holds the complete state tree of your application.
    /// </summary>
    /// <typeparam name="TState">The type of the root state tree.</typeparam>
    public sealed class Store<TState> : IStore<TState>, IObserverLinkedList<TState>
    {
        readonly IReducer<TState> _reducer;
        readonly IMiddlewareFactory<TState>[] _factories;
        readonly object _stateLock = new object();
        readonly object _buildLock = new object();
        readonly HashSet<Type> _building = new HashSet<Type>();
        // Published dictionaries are immutable. A cache hit needs no build lock.
        volatile Dictionary<Type, Delegate> _entries = new Dictionary<Type, Delegate>();
        ObserverNode<TState>? _root;
        ObserverNode<TState>? _last;

        internal Store(TState state, IReducer<TState> reducer, IMiddlewareFactory<TState>[] factories)
        {
            State = state;
            _reducer = reducer;
            _factories = factories;
        }

        /// <summary>Gets the current state of the store.</summary>
        /// <value>The initial state, or the state produced by the most recent successful reducer invocation.</value>
        public TState State
        {
            get;
            private set;
        }

        /// <summary>Dispatches an action through its cached, type-specific middleware pipeline.</summary>
        /// <typeparam name="TAction">The type of action to dispatch.</typeparam>
        /// <param name="action">The action passed to the middleware pipeline and reducer.</param>
        public void Dispatch<TAction>(TAction action)
        {
            Type type = typeof(TAction);
            Action<TAction> dispatch = _entries.TryGetValue(type, out Delegate entry)
                ? (Action<TAction>)entry
                : BuildEntry<TAction>();
            dispatch(action);
        }

        Action<TAction> BuildEntry<TAction>()
        {
            Type type = typeof(TAction);
            lock (_buildLock)
            {
                if (_entries.TryGetValue(type, out Delegate entry))
                {
                    return (Action<TAction>)entry;
                }

                if (!_building.Add(type))
                {
                    throw new InvalidOperationException("Recursive pipeline construction.");
                }

                try
                {
                    Func<TState, TAction, TState> reduce = _reducer.Invoke<TAction>;
                    Action<TAction> next = action =>
                    {
                        TState state;
                        lock (_stateLock)
                        {
                            state = reduce(State, action);
                            State = state;
                        }
                        OnNext(state);
                    };
                    for (var i = _factories.Length - 1; i >= 0; i--)
                    {
                        next = _factories[i].Build(this, next)
                            ?? throw new InvalidOperationException(
                                "Middleware returned a null dispatcher.");
                    }
                    _entries = new Dictionary<Type, Delegate>(_entries) { [type] = next };
                    return next;
                }
                finally
                {
                    _building.Remove(type);
                }
            }
        }

        /// <summary>
        /// Subscribes an observer and immediately notifies it of the current state.
        /// </summary>
        /// <param name="observer">
        /// The observer that receives the current state and subsequent state updates.
        /// </param>
        /// <returns>
        /// An <see cref="IDisposable"/> subscription whose disposal stops updates to this observer.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is null.</exception>
        public IDisposable Subscribe(IObserver<TState> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException(nameof(observer));
            }

            var next = new ObserverNode<TState>(this, observer);
            if (_root == null)
            {
                _root = next;
                _last = next;
            }
            else
            {
                _last!.Next = next;
                next.Previous = _last;
                _last = next;
            }
            next.OnNext(State);
            return next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void OnNext(TState value)
        {
            ObserverNode<TState>? node = _root;
            while (node != null)
            {
                node.OnNext(value);
                node = node.Next;
            }
        }

        void IObserverLinkedList<TState>.UnsubscribeNode(ObserverNode<TState> node)
        {
            if (node == _root)
            {
                _root = node.Next;
            }
            if (node == _last)
            {
                _last = node.Previous;
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
