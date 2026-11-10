namespace ReduxSharp
{
    /// <summary>Registers middleware and constructs stores.</summary>
    public interface IStoreBuilder<TState>
    {
        /// <summary>Appends a factory and returns this builder.</summary>
        IStoreBuilder<TState> Use(IMiddlewareFactory<TState> factory);

        /// <summary>Creates a store with the currently registered factories in registration order.</summary>
        Store<TState> Build(IReducer<TState> reducer);
    }
}
