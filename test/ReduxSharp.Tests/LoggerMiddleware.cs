using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class LoggerMiddleware<TState> : IMiddleware<TState>
    {
        readonly LoggerOptions options;

        public LoggerMiddleware(LoggerOptions options)
        {
            this.options = options;
        }

        public void Invoke<TAction>(IStore<TState> store, IDispatcher next, in TAction action)
        {
            options.Buffer.Add(action.GetType().FullName);
            next.Invoke(action);
        }
    }

    public class LoggerOptions
    {
        public List<string> Buffer { get; set; } = new List<string>();
    }
}
