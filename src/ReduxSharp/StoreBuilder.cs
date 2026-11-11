using System;
using System.Collections.Generic;

namespace ReduxSharp
{
    /// <summary>Configures a store with an initial state and an ordered middleware pipeline.</summary>
    /// <typeparam name="TState">The type of state held by the store.</typeparam>
    public sealed class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        readonly TState _initialState;
        readonly List<IMiddlewareFactory<TState>> _factories = new List<IMiddlewareFactory<TState>>();

        /// <summary>Creates a builder with the given initial state.</summary>
        /// <param name="initialState">The state supplied to each store created by this builder.</param>
        public StoreBuilder(TState initialState) => _initialState = initialState;

        /// <summary>Appends a middleware factory to the pipeline.</summary>
        /// <param name="factory">The factory used to build a stage for each store and action type.</param>
        /// <returns>This builder, allowing additional registrations.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/> is null.</exception>
        public IStoreBuilder<TState> Use(IMiddlewareFactory<TState> factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            _factories.Add(factory);
            return this;
        }

        /// <summary>Creates a store using a snapshot of the registered middleware factories.</summary>
        /// <param name="reducer">The reducer that computes the next state for each action.</param>
        /// <returns>A new store with its own state and action pipeline cache.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reducer"/> is null.</exception>
        public Store<TState> Build(IReducer<TState> reducer)
        {
            if (reducer == null)
            {
                throw new ArgumentNullException(nameof(reducer));
            }

            return new Store<TState>(_initialState, reducer, _factories.ToArray());
        }
    }
}
