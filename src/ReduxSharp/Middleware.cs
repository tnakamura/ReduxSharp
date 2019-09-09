using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// A higher-order function that composes a dispatch function to return a new dispatch function.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    /// <param name="store">A store</param>
    /// <param name="next">A dispatch function</param>
    /// <returns>A new dispatch function</returns>
    public delegate Dispatcher Middleware<TState>(IStore<TState> store, Dispatcher next);

    public interface IMiddleware<TState>
    {
        Task InvokeAsync<TAction>(IStore<TState> store, Func<TAction, Task> next, TAction action);
    }
}
