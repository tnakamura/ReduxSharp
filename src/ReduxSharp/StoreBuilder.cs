using System;
using System.Collections.Generic;

namespace ReduxSharp
{
    /// <summary>Configures a store with an initial state and an ordered middleware pipeline.</summary>
    public sealed class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        readonly TState initialState;
        readonly List<IMiddlewareFactory<TState>> factories = new List<IMiddlewareFactory<TState>>();

        /// <summary>Creates a builder with the given initial state.</summary>
        public StoreBuilder(TState initialState) => this.initialState = initialState;

        /// <inheritdoc/>
        public IStoreBuilder<TState> Use(IMiddlewareFactory<TState> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            factories.Add(factory);
            return this;
        }

        /// <inheritdoc/>
        public Store<TState> Build(IReducer<TState> reducer)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));
            return new Store<TState>(initialState, reducer, factories.ToArray());
        }
    }
}
