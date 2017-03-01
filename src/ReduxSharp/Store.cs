using ReduxSharp.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReduxSharp
{
    /// <summary>
    /// A store that holds the complete state tree of your application.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public class Store<TState> : IStore<TState>
    {
        readonly object _syncRoot = new object();

        readonly IReducer<TState> _reducer;

        readonly ListObserver<TState> _observer = new ListObserver<TState>();

        readonly DispatchDelegate _dispatch;

        /// <summary>
        /// Initializes a new instance of <see cref="Store{TState}"/> class.
        /// </summary>
        /// <param name="reducer">
        /// A reducing object that returns the next state tree.
        /// </param>
        /// <param name="initialState">
        /// The initial state.
        /// </param>
        /// <param name="middlewares">
        /// Functions that conform to the Redux middleware API.
        /// </param>
        public Store(IReducer<TState> reducer, TState initialState = default(TState), params MiddlewareDelegate<TState>[] middlewares)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

            _reducer = reducer;
            _dispatch = ApplyMiddlewares(middlewares);

            if (initialState != null)
            {
                State = initialState;
            }
            else
            {
                _dispatch(new ReduxInitialAction());
            }
        }

        /// <summary>
        /// Returns the current state tree of your application.
        /// </summary>
        public TState State { get; private set; }

        DispatchDelegate ApplyMiddlewares(IEnumerable<MiddlewareDelegate<TState>> middlewares)
        {
            return middlewares
                .Reverse()
                .Aggregate<MiddlewareDelegate<TState>, DispatchDelegate>(
                    InternalDispatch,
                    (next, middleware) => middleware(this, next));
        }

        /// <summary>
        /// Dispatches an action.
        /// </summary>
        /// <remarks>
        /// This is the only way to trigger a state change.
        /// </remarks>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        public void Dispatch(IAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            _dispatch(action);
        }

        void InternalDispatch(IAction action)
        {
            lock (_syncRoot)
            {
                var nextState = State = _reducer.Invoke(State, action);
                _observer.OnNext(nextState);
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

            lock (_syncRoot)
            {
                _observer.Add(observer);
                return new Subscription(this, observer);
            }
        }

        class Subscription : IDisposable
        {
            readonly object _lockObj = new object();

            Store<TState> _parent;

            IObserver<TState> _target;

            public Subscription(Store<TState> parent, IObserver<TState> target)
            {
                _parent = parent;
                _target = target;
            }

            public void Dispose()
            {
                lock (_lockObj)
                {
                    if (_parent != null)
                    {
                        lock (_parent._syncRoot)
                        {
                            _parent._observer.Remove(_target);
                        }
                    }
                    _target = null;
                    _parent = null;
                }
            }
        }
    }

    /// <summary>
    /// A function that accepts an action.
    /// </summary>
    /// <param name="action">
    /// An object describing the change that makes sense for your application.
    /// </param>
    public delegate void DispatchDelegate(IAction action);

    /// <summary>
    /// A higher-order function that composes a dispatch function to return a new dispatch function.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    /// <param name="store">A store</param>
    /// <param name="next">A dispatch function</param>
    /// <returns>A new dispatch function</returns>
    public delegate DispatchDelegate MiddlewareDelegate<TState>(IStore<TState> store, DispatchDelegate next);
}
