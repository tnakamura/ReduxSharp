using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Internal
{
    internal class ListObserver<T> : IObserver<T>
    {
        readonly List<IObserver<T>> observers;

        public ListObserver()
        {
            observers = new List<IObserver<T>>();
        }

        public void OnCompleted()
        {
            foreach (var observer in observers)
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var observer in observers)
            {
                observer.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(value);
            }
        }

        public IObserver<T> Add(IObserver<T> observer)
        {
            observers.Add(observer);
            return this;
        }

        public IObserver<T> Remove(IObserver<T> observer)
        {
            observers.Remove(observer);
            return this;
        }
    }
}
