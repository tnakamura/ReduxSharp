using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReduxSharp.Internal
{
    internal class ListObserver<T> : IObserver<T>
    {
        private readonly List<IObserver<T>> _observers;

        public ListObserver()
        {
            _observers = new List<IObserver<T>>();
        }

        public void OnCompleted()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            foreach (var observer in _observers)
            {
                observer.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        public IObserver<T> Add(IObserver<T> observer)
        {
            _observers.Add(observer);
            return this;
        }

        public IObserver<T> Remove(IObserver<T> observer)
        {
            _observers.Remove(observer);
            return this;
        }
    }
}
