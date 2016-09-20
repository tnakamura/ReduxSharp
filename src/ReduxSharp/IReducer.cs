namespace ReduxSharp
{
    public interface IReducer<TState>
    {
        TState Invoke(TState state, IAction action);
    }
}
