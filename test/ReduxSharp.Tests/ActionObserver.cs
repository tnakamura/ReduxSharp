using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Tests
{
    public class ActionObserver<T> : IObserver<T>
    {
        public Action Completed { get; set; }
        public Action<Exception> Error { get; set; }
        public Action<T> Next { get; set; }

        public void OnCompleted()
        {
            Completed?.Invoke();
        }

        public void OnError(Exception error)
        {
            Error?.Invoke(error);
        }

        public void OnNext(T value)
        {
            Next?.Invoke(value);
        }
    }
}
