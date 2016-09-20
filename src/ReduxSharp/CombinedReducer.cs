using System;
using System.Linq;

namespace ReduxSharp
{
    public class CombinedReducer<TState> : IReducer<TState>
    {
        private readonly IReducer<TState>[] reducers;

        public CombinedReducer(params IReducer<TState>[] reducers)
        {
            if (reducers == null) throw new ArgumentNullException(nameof(reducers));

            this.reducers = reducers;
        }

        public TState Invoke(TState state, IAction action)
        {
            return reducers.Aggregate(state, (currentState, reducer) => reducer.Invoke(currentState, action));
        }
    }
}
