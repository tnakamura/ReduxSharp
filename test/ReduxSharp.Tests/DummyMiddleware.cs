using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class DummyMiddleware<TState>
    {
        readonly IStore<TState> _store;
        readonly Dispatcher _next;

        public DummyMiddleware(IStore<TState> store, Dispatcher next)
        {
            _store = store;
            _next = next;
        }

        public void Invoke(IAction action)
        {
            _next(new DummyAction());
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
