using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// A function that creates and dispatches actions asynchronously.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    /// <param name="state">A state object</param>
    /// <param name="store">A store</param>
    /// <param name="callback">A dispatch function</param>
    /// <returns>A task that represents the asynchronous dispatch actions.</returns>
    public delegate Task AsyncActionCreatorDelegate<TState>(
        TState state,
        IStore<TState> store,
        Action<ActionCreatorDelegate<TState>> callback);
}
