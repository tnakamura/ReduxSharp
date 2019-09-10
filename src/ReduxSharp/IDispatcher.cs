using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// A function that accepts an action.
    /// </summary>
    /// <param name="action">
    /// An object describing the change that makes sense for your application.
    /// </param>
    [Obsolete]
    public delegate void Dispatcher(IAction action);

    /// <summary>
    /// An object that accepts an action.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Invoke an action.
        /// </summary>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        ValueTask Invoke<TAction>(TAction action);
    }
}
