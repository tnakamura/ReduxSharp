using ReduxSharp.Internal;
using System;
using System.Collections.Generic;

namespace ReduxSharp
{
    /// <summary>
    /// A builder for <see cref="Store{TState}"/>
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public sealed class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        readonly IReducer<TState> reducer;

        readonly List<IMiddleware<TState>> middlewares =
            new List<IMiddleware<TState>>();

        TState initialState = default;

        /// <summary>
        /// Initializes a new instance of <see cref="StoreBuilder{TState}"/> class.
        /// </summary>
        /// <param name="reducer">
        /// A reducing function that returns the next state tree.
        /// </param>
        public StoreBuilder(IReducer<TState> reducer)
        {
            this.reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
        }

        /// <summary>
        /// Add or replace an initial state.
        /// </summary>
        /// <param name="initialState">
        /// The initial state.
        /// </param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/>.</returns>
        public IStoreBuilder<TState> UseInitialState(TState initialState)
        {
            this.initialState = initialState;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="IStore{TState}"/>.
        /// </summary>
        /// <returns>The <see cref="IStore{TState}"/>.</returns>
        public IStore<TState> Build()
        {
            return new Store<TState>(
                reducer,
                initialState,
                middlewares.ToArray());
        }

        /// <summary>
        /// Adds a middleware instance to the store's dispatch pipeline.
        /// </summary>
        /// <param name="middleware">The middleware instance.</param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/> instance.</returns>
        public IStoreBuilder<TState> UseMiddleware(IMiddleware<TState> middleware)
        {
            if (middleware == null) throw new ArgumentNullException(nameof(middleware));

            middlewares.Add(middleware);

            return this;
        }
    }
}
