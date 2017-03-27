namespace ReduxSharp
{
    /// <summary>
    /// Accepts an accumulation and a value and returns a new accumulation.
    /// </summary>
    /// <typeparam name="TState">A type of state tree</typeparam>
    /// <param name="state">A state object</param>
    /// <param name="action">An action object</param>
    /// <returns>A new state object</returns>
    public delegate TState Reducer<TState>(TState state, IAction action);
}
