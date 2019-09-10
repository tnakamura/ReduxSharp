namespace ReduxSharp
{
    /// <summary>
    /// Initial action that is dispatched as soon as the store is created.
    /// </summary>
    /// <remarks>
    /// Reducers respond to this action by configuring thier initial state.
    /// </remarks>
#pragma warning disable CS0612
    public sealed class ReduxInitialAction : IAction
#pragma warning restore CS0612
    {
    }
}
