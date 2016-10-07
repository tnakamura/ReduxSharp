using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// A builder for <see cref="Store{TState}"/>
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        private readonly IReducer<TState> _reducer;

        private readonly List<MiddlewareDelegate<TState>> _middlewares
            = new List<MiddlewareDelegate<TState>>();

        private TState _initialState = default(TState);

        /// <summary>
        /// Initializes a new instance of <see cref="StoreBuilder{TState}"/> class.
        /// </summary>
        /// <param name="reducer">
        /// A reducing object that returns the next state tree.
        /// </param>
        public StoreBuilder(IReducer<TState> reducer)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

            _reducer = reducer;
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
            _initialState = initialState;
            return this;
        }

        /// <summary>
        /// Adds a middleware delegate to the store's dispatch pipeline.
        /// </summary>
        /// <param name="middleware">The middleware delegate.</param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/>.</returns>
        public IStoreBuilder<TState> UseMiddleware(MiddlewareDelegate<TState> middleware)
        {
            if (middleware == null) throw new ArgumentNullException(nameof(middleware));

            _middlewares.Add(middleware);
            return this;
        }

        /// <summary>
        /// Builds a <see cref="IStore{TState}"/>.
        /// </summary>
        /// <returns>The <see cref="IStore{TState}"/>.</returns>
        public IStore<TState> Build()
        {
            return new Store<TState>(_reducer, _initialState, _middlewares.ToArray());
        }
    }
}
