namespace ReduxSharp
{
    /// <summary>
    /// A function that accepts an action.
    /// </summary>
    /// <param name="action">
    /// An object describing the change that makes sense for your application.
    /// </param>
    public delegate void DispatchFunction(IAction action);
}
