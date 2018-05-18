using ReduxSharp.Internal;
using System;
using System.Collections.Generic;

namespace ReduxSharp
{
    /// <summary>
    /// A builder for <see cref="Store{TState}"/>
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        readonly Reducer<TState> reducer;

        readonly List<Middleware<TState>> middlewares
            = new List<Middleware<TState>>();

        TState initialState = default(TState);

        /// <summary>
        /// Initializes a new instance of <see cref="StoreBuilder{TState}"/> class.
        /// </summary>
        /// <param name="reducer">
        /// A reducing function that returns the next state tree.
        /// </param>
        public StoreBuilder(Reducer<TState> reducer)
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
        /// Adds a middleware delegate to the store's dispatch pipeline.
        /// </summary>
        /// <param name="middleware">The middleware delegate.</param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/> instance.</returns>
        public IStoreBuilder<TState> UseMiddleware(Middleware<TState> middleware)
        {
            if (middleware == null) throw new ArgumentNullException(nameof(middleware));

            middlewares.Add(middleware);
            return this;
        }

        /// <summary>
        /// Builds a <see cref="IStore{TState}"/>.
        /// </summary>
        /// <returns>The <see cref="IStore{TState}"/>.</returns>
        public IStore<TState> Build()
        {
            return new Store<TState>(reducer, initialState, middlewares.ToArray());
        }

        /// <summary>
        /// Adds a middleware type to the store's dispatch pipeline.
        /// </summary>
        /// <typeparam name="TMiddleware">The middleware type.</typeparam>
        /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/> instance.</returns>
        public IStoreBuilder<TState> UseMiddleware<TMiddleware>(params object[] args)
        {
            return UseMiddleware(typeof(TMiddleware), args);
        }

        /// <summary>
        /// Adds a middleware type to the store's dispatch pipeline.
        /// </summary>
        /// <param name="middleware">The middleware type.</param>
        /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/> instance.</returns>
        public IStoreBuilder<TState> UseMiddleware(Type middleware, params object[] args)
        {
            if (middleware == null) throw new ArgumentNullException(nameof(middleware));

            return UseMiddleware(MiddlewareFactory.Create<TState>(middleware, args));
        }
    }
}
