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

		public async ValueTask Invoke<TAction>(IStore<TState> store, IDispatcher next, TAction action)
		{
			options.Buffer.Add(action.GetType().FullName);
			await next.Invoke(action);
		}
	}

    public class LoggerOptions
    {
        public List<string> Buffer { get; set; } = new List<string>();
    }
}
