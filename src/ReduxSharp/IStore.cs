using System;

namespace ReduxSharp
{
    /// <summary>
    /// Represents a store that holds the complete state tree of your application.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public interface IStore<TState> : IObservable<TState>
    {
        /// <summary>
        /// Returns the current state tree of your application.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// Dispatches an action.
        /// </summary>
        /// <remarks>
        /// This is the only way to trigger a state change.
        /// </remarks>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        void Dispatch(IAction action);
    }
}
