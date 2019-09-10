using System;
using System.Threading.Tasks;

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
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        [Obsolete]
        void Dispatch(IAction action);

        /// <summary>
        /// Dispatches an action creator.
        /// </summary>
        /// <param name="actionCreator">
        /// A function that creates an action.
        /// </param>
        [Obsolete]
        void Dispatch(ActionCreator<TState> actionCreator);

        /// <summary>
        /// Dispatches an async action creator.
        /// </summary>
        /// <param name="asyncActionCreator">
        /// A function that creates and dispatches actions asynchronously.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        [Obsolete]
        Task Dispatch(AsyncActionCreator<TState> asyncActionCreator);

        /// <summary>
        /// Dispatches an action.
        /// </summary>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        ValueTask Dispatch<TAction>(TAction action);
    }
}
