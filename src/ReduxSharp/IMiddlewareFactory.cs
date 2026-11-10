using System;

namespace ReduxSharp
{
    /// <summary>Builds one stage of a store's pipeline for each dispatched action type.</summary>
    public interface IMiddlewareFactory<TState>
    {
        /// <summary>Wraps the remaining pipeline. The returned delegate is reused for this store and action type.</summary>
        Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next);
    }
}
