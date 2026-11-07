using System;
using Benchmark.Prototype;
using ReduxSharp;

namespace Benchmark.Examples
{
    public static class LoggingMiddlewareExtensions
    {
        public static IStoreBuilder<TState> UseLoggingMiddleware<TState>(
            this IStoreBuilder<TState> builder, Action<string> write = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.Use(new LoggingMiddleware<TState>(write ?? Console.WriteLine));
        }

        sealed class LoggingMiddleware<TState>(Action<string> write) : IMiddlewareFactory<TState>
        {
            public Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next) => action =>
            {
                write($"Dispatching {typeof(TAction).Name}");
                next(action);
            };
        }
    }
}
