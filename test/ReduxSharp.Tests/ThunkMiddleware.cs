using System;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class ThunkMiddleware<TState> : IMiddleware<TState>
    {
        public async ValueTask Invoke<TAction>(IStore<TState> store, IDispatcher next, TAction action)
        {
            switch (action)
            {
                case Func<IDispatcher, ValueTask> asyncAction:
                    await asyncAction(next);
                    break;
                case Func<IDispatcher, Task> asyncAction:
                    await asyncAction(next);
                    break;
                default:
                    await next.Invoke(action);
                    break;
            }
        }
    }
}
