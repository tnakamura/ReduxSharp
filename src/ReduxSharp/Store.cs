using System;
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
        readonly DispatchPipeline dispatcher;

        readonly AsyncLock writeLock = new AsyncLock();

        ObserverNode<TState> root;

        ObserverNode<TState> last;

		/// <summary>
		/// Initializes a new instance of <see cref="Store{TState}"/> class.
		/// </summary>
		/// <param name="reducer">
		/// A reducing function that returns the next state tree.
		/// </param>
		/// <param name="state">
		/// The initial state.
		/// </param>
		/// <param name="middlewares">
		/// Functions that conform to the Redux middleware API.
		/// </param>
		public Store(IReducer<TState> reducer, TState state, params IMiddleware<TState>[] middlewares)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

			State = state;
            dispatcher = new DispatchPipeline(this, reducer, middlewares);
        }

        /// <summary>
        /// Returns the current state tree of your application.
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        /// Dispatches an action.
        /// </summary>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        public async ValueTask Dispatch<TAction>(TAction action)
        {
            await dispatcher.Invoke(action)
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
