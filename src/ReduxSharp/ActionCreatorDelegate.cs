namespace ReduxSharp
{
    public delegate IAction ActionCreatorDelegate<TState>(TState state, IStore<TState> store);
}
