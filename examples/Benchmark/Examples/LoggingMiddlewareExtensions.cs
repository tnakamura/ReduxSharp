using System;
using ReduxSharp;

namespace Benchmark.Examples
{
    /// <summary>Registers middleware that logs dispatched action names.</summary>
    public static class LoggingMiddlewareExtensions
    {
        /// <summary>Appends a logging middleware factory to the builder.</summary>
        /// <typeparam name="TState">The type of state held by the store.</typeparam>
        /// <param name="builder">The builder that receives the middleware registration.</param>
        /// <param name="write">The log output callback, or null to write to the console.</param>
        /// <returns>The builder, allowing additional registrations.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public static IStoreBuilder<TState> UseLoggingMiddleware<TState>(
            this IStoreBuilder<TState> builder, Action<string> write = null)
        {
            ArgumentNullException.ThrowIfNull(builder);
            return builder.Use(new LoggingMiddleware<TState>(write ?? Console.WriteLine));
        }

        sealed class LoggingMiddleware<TState>(Action<string> write) : IMiddlewareFactory<TState>
        {
            /// <summary>Builds a delegate that logs the action name before continuing the pipeline.</summary>
            /// <typeparam name="TAction">The type of action handled by this pipeline.</typeparam>
            /// <param name="store">The store whose pipeline is being built.</param>
            /// <param name="next">The delegate that continues the pipeline.</param>
            /// <returns>The logging delegate for this action type.</returns>
            public Action<TAction> Build<TAction>(IStore<TState> store, Action<TAction> next) => action =>
            {
                write($"Dispatching {typeof(TAction).Name}");
                next(action);
            };
        }
    }
}
