using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class DummyMiddleware<TState> : IMiddleware<TState>
    {
        public void Invoke<TAction>(IStore<TState> store, IDispatcher next, in TAction action)
        {
            next.Invoke(new DummyAction());
        }
    }

    public class DummyAction
    {
    }
}
