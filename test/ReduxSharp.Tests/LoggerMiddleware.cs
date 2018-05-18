using System.Collections.Generic;

namespace ReduxSharp.Tests
{
    public class LoggerMiddleware<TState>
    {
        readonly IStore<TState> store;
        readonly Dispatcher next;
        readonly LoggerOptions options;

        public LoggerMiddleware(IStore<TState> store, Dispatcher next, LoggerOptions options)
        {
            this.store = store;
            this.next = next;
            this.options = options;
        }

        public void Invoke(IAction action)
        {
            options.Buffer.Add(action.GetType().FullName);
            next(action);
        }
    }

    public class LoggerOptions
    {
        public List<string> Buffer { get; set; } = new List<string>();
    }

    public static class LoggerMiddlewareExtensions
    {
        public static IStoreBuilder<TState> UseLogger<TState>(this IStoreBuilder<TState> store, LoggerOptions options)
        {
            return store.UseMiddleware<LoggerMiddleware<TState>>(options);
        }
    }
}
