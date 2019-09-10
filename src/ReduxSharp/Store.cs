using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ReduxSharp.Internal;

namespace ReduxSharp
{
    /// <summary>
    /// A store that holds the complete state tree of your application.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public sealed class Store<TState> : IStore<TState>, IObserverLinkedList<TState>
    {
        readonly object syncRoot = new object();

        [Obsolete]
        readonly Reducer<TState> reducer;

        [Obsolete]
        readonly Dispatcher dispatcher;

        readonly DispatchPipeline internalDispatcher;

        readonly AsyncLock writeLock = new AsyncLock();

        ObserverNode<TState> root;

        ObserverNode<TState> last;

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
        [Obsolete]
        public Store(Reducer<TState> reducer, TState initialState = default, params Middleware<TState>[] middlewares)
        {
            this.reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
            dispatcher = ApplyMiddlewares(middlewares);

            if (initialState != default)
            {
                State = initialState;
            }
            else
            {
                dispatcher(new ReduxInitialAction());
            }
        }

        internal Store(IReducer<TState> reducer, TState initialState, params IMiddleware<TState>[] middlewares)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

            internalDispatcher = new DispatchPipeline(this, reducer, middlewares);
            if (initialState != default)
            {
                State = initialState;
            }
            else
            {
                Dispatch(new ReduxInitialAction())
                    .GetAwaiter()
                    .GetResult();
            }
        }

        /// <summary>
        /// Returns the current state tree of your application.
        /// </summary>
        public TState State { get; private set; }

        [Obsolete]
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
        [Obsolete]
        public void Dispatch(IAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            try
            {
                dispatcher(action);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        [Obsolete]
        void InternalDispatch(IAction action)
        {
            lock (syncRoot)
            {
                var nextState = State = reducer(State, action);
                OnNext(nextState);
            }
        }

        /// <summary>
        /// Dispatches an action creator.
        /// </summary>
        /// <param name="actionCreator">
        /// A function that creates an action.
        /// </param>
        [Obsolete]
        public void Dispatch(ActionCreator<TState> actionCreator)
        {
            if (actionCreator == null) throw new ArgumentNullException(nameof(actionCreator));

            try
            {
                var action = actionCreator(State, this);
                if (action != null)
                {
                    dispatcher(action);
                }
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// Dispatches an async action creator.
        /// </summary>
        /// <param name="asyncActionCreator">
        /// A function that creates and dispatches actions asynchronously.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        [Obsolete]
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
                        dispatcher(action);
                    }
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        /// <summary>
        /// Dispatches an action.
        /// </summary>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        public async ValueTask Dispatch<TAction>(TAction action)
        {
            await internalDispatcher.Invoke(action)
                .ConfigureAwait(false);
        }

        sealed class MiddlewareDispatcher : IDispatcher
        {
            readonly IMiddleware<TState> middleware;

            readonly IDispatcher next;

            readonly IStore<TState> store;

            public MiddlewareDispatcher(IMiddleware<TState> middleware, IStore<TState> store, IDispatcher next)
            {
                this.store = store;
                this.next = next;
                this.middleware = middleware;
            }

            public async ValueTask Invoke<TAction>(TAction action)
            {
                await middleware.Invoke(store, next, action).ConfigureAwait(false);
            }
        }

        sealed class ActionDispatcher : IDispatcher
        {
            readonly IReducer<TState> reducer;

            readonly Store<TState> store;

            public ActionDispatcher(Store<TState> store, IReducer<TState> reducer)
            {
                this.store = store;
                this.reducer = reducer;
            }

            public async ValueTask Invoke<TAction>(TAction action)
            {
                using (await store.writeLock.LockAsync())
                {
                    try
                    {
                        var currentState = store.State;
                        var nextState = await reducer.Invoke(currentState, action)
                            .ConfigureAwait(false);
                        store.State = nextState;
                        store.OnNext(store.State);
                    }
                    catch (Exception ex)
                    {
                        store.OnError(ex);
                    }
                }
            }
        }

        sealed class DispatchPipeline : IDispatcher
        {
            readonly IDispatcher innerDispatcher;

            public DispatchPipeline(
                Store<TState> store,
                IReducer<TState> reducer,
                IMiddleware<TState>[] middlewares)
            {
                IDispatcher next = new ActionDispatcher(store, reducer);
                foreach (var middleware in middlewares.Reverse())
                {
                    next = new MiddlewareDispatcher(middleware, store, next);
                }
                innerDispatcher = next;
            }

            public async ValueTask Invoke<TAction>(TAction action)
            {
                await innerDispatcher.Invoke(action).ConfigureAwait(false);
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
                last.Next = next;
                next.Previous = last;
                last = next;
            }
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void OnError(Exception error)
        {
            var node = root;
            while (node != null)
            {
                node.OnError(error);
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
