using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// Defines a middleware.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public interface IMiddleware<TState>
    {
        /// <summary>
        /// Accepts a middleware.
        /// </summary>
        /// <typeparam name="TAction">A type of action</typeparam>
        /// <param name="store">A store</param>
        /// <param name="next">A dispatch function</param>
        /// <param name="action">An action object</param>
        ValueTask Invoke<TAction>(IStore<TState> store, IDispatcher next, TAction action);
    }
}
