using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class DummyMiddleware<TState>
    {
        private readonly IStore<TState> _store;
        private readonly DispatchDelegate _next;

        public DummyMiddleware(IStore<TState> store, DispatchDelegate next)
        {
            _store = store;
            _next = next;
        }

        public IAction Invoke(IAction action)
        {
            return _next(new DummyAction());
        }
    }

    public class DummyAction : IAction
    {
    }

    public static class DummyMiddlewareExtensions
    {
        public static IStoreBuilder<TState> UseDummy<TState>(this IStoreBuilder<TState> store)
        {
            return store.UseMiddleware<DummyMiddleware<TState>>();
        }
    }
}
