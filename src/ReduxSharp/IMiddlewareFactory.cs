using System;

namespace ReduxSharp
{
    /// <summary>Builds one stage of a store's pipeline for each dispatched action type.</summary>
    /// <typeparam name="TState">The type of state held by the store.</typeparam>
    public interface IMiddlewareFactory<TState>
    {
        /// <summary>Wraps the remaining pipeline. The returned delegate is reused for this store and action type.</summary>
        /// <typeparam name="TAction">The type of action handled by the pipeline.</typeparam>
        /// <param name="store">The store whose pipeline is being built.</param>
        /// <param name="next">The delegate that continues the pipeline with an action of this type.</param>
        /// <returns>The delegate that handles the action and optionally invokes the remaining pipeline.</returns>
        Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next);
    }
}
