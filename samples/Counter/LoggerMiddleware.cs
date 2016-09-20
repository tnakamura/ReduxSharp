using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp;

namespace Counter
{
    public static class LoggerMiddleware
    {
        public static DispatchDelegate Invoke<TState>(IStore<TState> store, DispatchDelegate next)
        {
            return new DispatchDelegate(action =>
            {
                Console.WriteLine(action.GetType().FullName);
                return next(action);
            });
        }
    }
}
