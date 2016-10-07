namespace ReduxSharp
{
    public interface IStoreBuilder<TState>
    {
        Store<TState> Build();

        IStoreBuilder<TState> InitialState(TState state);

        IStoreBuilder<TState> Use(MiddlewareDelegate<TState> middleware);
    }
}