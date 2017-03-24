using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp.Internal;

namespace ReduxSharp
{
    /// <summary>
    /// A builder for <see cref="Store{TState}"/>
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        readonly IReducer<TState> _reducer;

        readonly List<Middleware<TState>> _middlewares
            = new List<Middleware<TState>>();

        TState _initialState = default(TState);

        /// <summary>
        /// Initializes a new instance of <see cref="StoreBuilder{TState}"/> class.
        /// </summary>
        /// <param name="reducer">
        /// A reducing object that returns the next state tree.
        /// </param>
        public StoreBuilder(IReducer<TState> reducer)
        {
            _reducer = reducer ?? throw new ArgumentNullException(nameof(reducer));
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
        /// <returns>The <see cref="IStoreBuilder{TState}"/> instance.</returns>
        public IStoreBuilder<TState> UseMiddleware(Middleware<TState> middleware)
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
