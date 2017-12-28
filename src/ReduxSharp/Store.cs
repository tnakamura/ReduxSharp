using ReduxSharp.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// A store that holds the complete state tree of your application.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public partial class Store<TState> : IStore<TState>
    {
        readonly object _syncRoot = new object();

        readonly Reducer<TState> _reducer;

        readonly ListObserver<TState> _observer = new ListObserver<TState>();

        readonly Dispatcher _dispatcher;

        /// <summary>
        /// Initializes a new instance of <see cref="Store{TState}"/> class.
        /// </summary>
        /// <param name="reducer">
        /// A reducing function that returns the next state tree.
        /// </param>
        /// <param name="initialState">
        /// The initial state.
        /// </param>
        /// <param name="middlewares">
        /// Functions that conform to the Redux middleware API.
        /// </param>
        public Store(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] middlewares)
        {
            _reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
            _dispatcher = ApplyMiddlewares(middlewares);

            if (initialState != null)
            {
                State = initialState;
            }
            else
            {
                _dispatcher(new ReduxInitialAction());
            }
        }

        /// <summary>
        /// Returns the current state tree of your application.
        /// </summary>
        public TState State { get; private set; }

        Dispatcher ApplyMiddlewares(IEnumerable<Middleware<TState>> middlewares)
        {
            return middlewares
                .Reverse()
                .Aggregate<Middleware<TState>, Dispatcher>(
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

            try
            {
                _dispatcher(action);
            }
            catch (Exception ex)
            {
                _observer.OnError(ex);
            }
        }

        void InternalDispatch(IAction action)
        {
            lock (_syncRoot)
            {
                var nextState = State = _reducer(State, action);
                _observer.OnNext(nextState);
            }
        }

        /// <summary>
        /// Dispatches an action creator.
        /// </summary>
        /// <param name="actionCreator">
        /// A function that creates an action.
        /// </param>
        public void Dispatch(ActionCreator<TState> actionCreator)
        {
            if (actionCreator == null) throw new ArgumentNullException(nameof(actionCreator));

            try
            {
                var action = actionCreator(State, this);
                if (action != null)
                {
                    _dispatcher(action);
                }
            }
            catch (Exception ex)
            {
                _observer.OnError(ex);
            }
        }

        /// <summary>
        /// Dispatches an async action creator.
        /// </summary>
        /// <param name="asyncActionCreator">
        /// A function that creates and dispatches actions asynchronously.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        public async Task Dispatch(AsyncActionCreator<TState> asyncActionCreator)
        {
            if (asyncActionCreator == null) throw new ArgumentNullException(nameof(asyncActionCreator));

            try
            {
                await asyncActionCreator(State, this, actionCreator =>
                {
                    var action = actionCreator(State, this);
                    if (action != null)
                    {
                        _dispatcher(action);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _observer.OnError(ex);
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
    }
}
