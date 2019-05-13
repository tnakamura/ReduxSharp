using System;
using System.Threading;

namespace ReduxSharp.Internal
{
    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        readonly IObserver<T> observer;

        IObserverLinkedList<T> list;

        internal ObserverNode<T> Previous { get; set; }

        internal ObserverNode<T> Next { get; set; }

        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            this.list = list;
            this.observer = observer;
        }

        public void OnCompleted()
        {
            observer.OnCompleted();
        }

        public void OnError(Exception error)
        {
            observer.OnError(error);
        }

        public void OnNext(T value)
        {
            observer.OnNext(value);
        }

        public void Dispose()
        {
            var sourceList = Interlocked.Exchange(ref list, null);
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
            }
        }
    }
}
