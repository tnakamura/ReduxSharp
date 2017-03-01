using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ReduxSharp.Internal
{
    internal static class MiddlewareFactory
    {
        private const string InvokeMethodName = "Invoke";

        public static MiddlewareDelegate<TState> Create<TState>(Type middleware, object[] args)
        {
            return (store, next) =>
            {
                var typeInfo = middleware.GetTypeInfo();

                var invokeMethod = typeInfo.GetDeclaredMethod(InvokeMethodName);
                if (invokeMethod == null)
                {
                    throw new InvalidOperationException($"{middleware.Name} require {InvokeMethodName} method");
                }

                var invokeParameters = invokeMethod.GetParameters();
                if (invokeParameters.Length == 0)
                {
                    throw new InvalidOperationException($"{InvokeMethodName} required action argument");
                }

                var ctorArgs = new object[args.Length + 2];
                ctorArgs[0] = store;
                ctorArgs[1] = next;
                Array.Copy(args, 0, ctorArgs, 2, args.Length);
                var instance = Activator.CreateInstance(middleware, ctorArgs);

                var factory = Compile<object>(invokeMethod, invokeParameters);

                return action => factory(instance, action);
            };
        }

        private static Action<T, IAction> Compile<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
        {
            var middleware = typeof(T);
            var actionArg = Expression.Parameter(typeof(IAction), "action");
            var instanceArg = Expression.Parameter(middleware, "instance");

            // Invoke method's argument is action only.
            var methodArguments = new Expression[parameters.Length];
            methodArguments[0] = actionArg;

            Expression middleweareInstanceArg = instanceArg;
            if (methodInfo.DeclaringType != typeof(T))
            {
                middleweareInstanceArg = Expression.Convert(middleweareInstanceArg, methodInfo.DeclaringType);
            }

            var body = Expression.Call(middleweareInstanceArg, methodInfo, methodArguments);

            var lambda = Expression.Lambda<Action<T, IAction>>(body, instanceArg, actionArg);

            return lambda.Compile();
        }
    }
}
