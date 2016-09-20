using System;

namespace ReduxSharp
{
    public interface IStore<TState> : IObservable<TState>
    {
        TState State { get; }

        IAction Dispatch(IAction action);
    }
}
