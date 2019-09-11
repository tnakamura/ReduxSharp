namespace ReduxSharp
{
    /// <summary>
    /// Represents a dispatcher that accepts an action.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Accepts an action.
        /// </summary>
        /// <param name="action">
        /// An object describing the change that makes sense for your application.
        /// </param>
        void Invoke<TAction>(in TAction action);
    }
}
