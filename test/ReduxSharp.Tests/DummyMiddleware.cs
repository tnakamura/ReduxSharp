using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class DummyMiddleware<TState> : IMiddleware<TState>
    {
		public async ValueTask Invoke<TAction>(IStore<TState> store, IDispatcher next, TAction action)
		{
			await next.Invoke(new DummyAction());
		}
	}

    public class DummyAction : IAction
    {
    }

    public static class DummyMiddlewareExtensions
    {
        public static IStoreBuilder<TState> UseDummy<TState>(this IStoreBuilder<TState> store)
        {
            return store.UseMiddleware(new DummyMiddleware<TState>());
        }
    }
}
