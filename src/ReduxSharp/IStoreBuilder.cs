namespace ReduxSharp
{
    /// <summary>
    /// A builder for <see cref="Store{TState}"/>
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public interface IStoreBuilder<TState>
    {
        /// <summary>
        /// Builds a <see cref="IStore{TState}"/>.
        /// </summary>
        /// <returns>The <see cref="IStore{TState}"/></returns>
        IStore<TState> Build();

        /// <summary>
        /// Add or replace an initial state.
        /// </summary>
        /// <param name="initialState">
        /// The initial state.
        /// </param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/>.</returns>
        IStoreBuilder<TState> UseInitialState(TState initialState);

        /// <summary>
        /// Adds a middleware delegate to the store's dispatch pipeline.
        /// </summary>
        /// <param name="middleware">The middleware delegate.</param>
        /// <returns>The <see cref="IStoreBuilder{TState}"/>.</returns>
        IStoreBuilder<TState> Use(MiddlewareDelegate<TState> middleware);
    }
}