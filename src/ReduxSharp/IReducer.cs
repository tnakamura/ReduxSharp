using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// Accepts an accumulation and a value and returns a new accumulation.
    /// </summary>
    /// <typeparam name="TState">A type of state tree</typeparam>
    /// <param name="state">A state object</param>
    /// <param name="action">An action object</param>
    /// <returns>A new state object</returns>
    [Obsolete]
    public delegate TState Reducer<TState>(TState state, IAction action);

    /// <summary>
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
        /// <returns>A new state object</returns>
        ValueTask<TState> Invoke<TAction>(TState state, TAction action);
    }
}
