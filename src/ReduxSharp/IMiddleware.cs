using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    public interface IMiddleware<TState>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TAction">A type of action</typeparam>
        /// <param name="store">A store</param>
        /// <param name="next">A dispatch function</param>
        /// <param name="action">An action object</param>
        /// <returns></returns>
        ValueTask Invoke<TAction>(IStore<TState> store, IDispatcher next, TAction action);
    }
}
