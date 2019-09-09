using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// A function that accepts an action.
    /// </summary>
    /// <param name="action">
    /// An object describing the change that makes sense for your application.
    /// </param>
    public delegate void Dispatcher(IAction action);

    public interface IDispatcher
    {
        ValueTask Invoke<TAction>(TAction action);
    }
}
