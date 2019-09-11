﻿using System;
using System.Linq;
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
        readonly IDispatcher dispatcher;

        readonly object dispatchLock = new object();

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
        /// Objects that conform to the Redux middleware API.
        /// </param>
        public Store(IReducer<TState> reducer, TState state, params IMiddleware<TState>[] middlewares)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

            State = state;
			dispatcher = ApplyMiddlewares(reducer, middlewares);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IDispatcher ApplyMiddlewares(IReducer<TState> reducer, IMiddleware<TState>[] middlewares)
		{
			IDispatcher next = new ActionDispatcher(this, reducer);
			foreach (var middleware in middlewares.Reverse())
			{
				next = new MiddlewareDispatcher(middleware, this, next);
			}
			return next;
		}

        /// <summary>
        /// Returns the current state tree of your application.
        /// </summary>
        public TState State { get; private set; }

        /// <summary>
        /// Dispatches an action.
        /// </summary>
		/// <typeparam name="TAction">A type of action.</typeparam>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        public void Dispatch<TAction>(in TAction action) => dispatcher.Invoke(action);

        sealed class MiddlewareDispatcher : IDispatcher
        {
            readonly IMiddleware<TState> middleware;

            readonly IDispatcher next;

            readonly IStore<TState> store;

            public MiddlewareDispatcher(
                IMiddleware<TState> middleware,
                IStore<TState> store,
                IDispatcher next)
            {
                this.store = store;
                this.next = next;
                this.middleware = middleware;
            }

            public void Invoke<TAction>(in TAction action) =>
                middleware.Invoke(store, next, action);
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

            public void Invoke<TAction>(in TAction action)
            {
                lock (store.dispatchLock)
                {
                    var currentState = store.State;
                    var nextState = reducer.Invoke(currentState, action);
                    store.State = nextState;
                    store.OnNext(store.State);
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
