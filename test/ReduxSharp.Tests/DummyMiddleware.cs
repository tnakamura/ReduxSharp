using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class DummyMiddleware<TState>
    {
        readonly Func<TState> _getState;
        readonly DispatchDelegate _next;

        public DummyMiddleware(Func<TState> getState, DispatchDelegate next)
        {
            _getState = getState;
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
