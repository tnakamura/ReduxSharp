namespace ReduxSharp
{
    /// <summary>
    /// Defines a reducer that accepts an accumulation
    /// and a value and returns a new accumulation.
    /// </summary>
    /// <typeparam name="TState">The type of the state tree.</typeparam>
    public interface IReducer<TState>
    {
        /// <summary>
        /// Accepts an accumulation and a value and returns a new accumulation.
        /// </summary>
        /// <typeparam name="TAction">The type of the action.</typeparam>
        /// <param name="state">The state object.</param>
        /// <param name="action">The action object.</param>
        /// <returns>The next state after applying the action to the supplied state.</returns>
        TState Invoke<TAction>(TState state, TAction action);
    }
}
