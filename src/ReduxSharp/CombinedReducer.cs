using System;
using System.Linq;

namespace ReduxSharp
{
    /// <summary>
    /// A reducer that combines multiple reducers into one.
    /// </summary>
    /// <remarks>
    /// You will typically use this reducer during initial store setup.
    /// </remarks>
    /// <typeparam name="TState"></typeparam>
    public class CombinedReducer<TState> : IReducer<TState>
    {
        readonly IReducer<TState>[] _reducers;

        /// <summary>
        /// Initializes a new instance of <see cref="CombinedReducer{TState}"/> class.
        /// </summary>
        /// <param name="reducers">the list of reducers</param>
        public CombinedReducer(params IReducer<TState>[] reducers)
        {
            _reducers = reducers ?? throw new ArgumentNullException(nameof(reducers));
        }

        /// <summary>
        /// Accepts an accumulation and a value and returns a new accumulation.
        /// </summary>
        /// <param name="state">A state object</param>
        /// <param name="action">An action object</param>
        /// <returns>A new state object</returns>
        public TState Invoke(TState state, IAction action)
        {
            return _reducers.Aggregate(state, (currentState, reducer) => reducer.Invoke(currentState, action));
        }
    }
}
