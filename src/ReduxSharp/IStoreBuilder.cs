namespace ReduxSharp
{
    /// <summary>Registers middleware and constructs stores.</summary>
    /// <typeparam name="TState">The type of state held by the store.</typeparam>
    public interface IStoreBuilder<TState>
    {
        /// <summary>Appends a factory and returns this builder.</summary>
        /// <param name="factory">The factory to append in registration order.</param>
        /// <returns>This builder, allowing additional registrations.</returns>
        IStoreBuilder<TState> Use(IMiddlewareFactory<TState> factory);

        /// <summary>Creates a store with the currently registered factories in registration order.</summary>
        /// <param name="reducer">The reducer that computes the next state for each action.</param>
        /// <returns>A new store using a snapshot of the registered factories.</returns>
        Store<TState> Build(IReducer<TState> reducer);
    }
}
