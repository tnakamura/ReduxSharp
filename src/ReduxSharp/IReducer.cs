namespace ReduxSharp
{
    /// <summary>
    /// Defines a reducer that accepts an accumulation
    /// and a value and returns a new accumulation.
    /// </summary>
    /// <typeparam name="TState">A type of state tree</typeparam>
    public interface IReducer<TState>
    {
        /// <summary>
        /// Accepts an accumulation and a value and returns a new accumulation.
        /// </summary>
        /// <typeparam name="TAction">A type of action</typeparam>
        /// <param name="state">A state object</param>
        /// <param name="action">An action object</param>
        /// <returns>The next state after applying the action to the supplied state.</returns>
        TState Invoke<TAction>(TState state, TAction action);
    }
}
