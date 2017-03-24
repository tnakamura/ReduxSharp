using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class LoggerMiddleware<TState>
    {
        readonly IStore<TState> _store;
        readonly DispatchFunction _next;
        readonly LoggerOptions _options;

        public LoggerMiddleware(IStore<TState> store, DispatchFunction next, LoggerOptions options)
        {
            _store = store;
            _next = next;
            _options = options;
        }

        public void Invoke(IAction action)
        {
            _options.Buffer.Add(action.GetType().FullName);
            _next(action);
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
