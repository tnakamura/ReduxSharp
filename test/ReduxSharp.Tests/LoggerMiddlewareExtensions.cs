using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public static class LoggerMiddlewareExtensions
    {
        public static IStoreBuilder<TState> UseLogger<TState>(this IStoreBuilder<TState> store, LoggerOptions options)
        {
            return store.UseMiddleware<LoggerMiddleware<TState>>(options);
        }
    }
}
