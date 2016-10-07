using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp
{
    public class StoreBuilder<TState> : IStoreBuilder<TState>
    {
        private readonly IReducer<TState> _reducer;

        private readonly List<MiddlewareDelegate<TState>> _middlewares
            = new List<MiddlewareDelegate<TState>>();

        private TState _initialState = default(TState);

        public StoreBuilder(IReducer<TState> reducer)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

            _reducer = reducer;
        }

        public IStoreBuilder<TState> InitialState(TState state)
        {
            _initialState = state;
            return this;
        }

        public IStoreBuilder<TState> Use(MiddlewareDelegate<TState> middleware)
        {
            if (middleware == null) throw new ArgumentNullException(nameof(middleware));

            _middlewares.Add(middleware);
            return this;
        }

        public Store<TState> Build()
        {
            return new Store<TState>(_reducer, _initialState, _middlewares.ToArray());
        }
    }
}
