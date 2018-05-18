namespace ReduxSharp.Tests
{
    public class DummyMiddleware<TState>
    {
        readonly IStore<TState> store;
        readonly Dispatcher next;

        public DummyMiddleware(IStore<TState> store, Dispatcher next)
        {
            this.store = store;
            this.next = next;
        }

        public void Invoke(IAction action)
        {
            next(new DummyAction());
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
