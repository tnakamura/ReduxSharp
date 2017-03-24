namespace ReduxSharp
{
    /// <summary>
    /// A function that creates an action.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    /// <param name="state">A state object</param>
    /// <param name="store">A store</param>
    /// <returns>An action object</returns>
    public delegate IAction ActionCreator<TState>(TState state, IStore<TState> store);
}
