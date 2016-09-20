namespace ReduxSharp
{
    /// <summary>
    /// Represents a reducing object.
    /// </summary>
    /// <typeparam name="TState">A type of state tree</typeparam>
    public interface IReducer<TState>
    {
        /// <summary>
        /// Accepts an accumulation and a value and returns a new accumulation.
        /// </summary>
        /// <param name="state">A state object</param>
        /// <param name="action">An action object</param>
        /// <returns>A new state object</returns>
        TState Invoke(TState state, IAction action);
    }
}
