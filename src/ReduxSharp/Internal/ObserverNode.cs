using System;
using System.Threading;

namespace ReduxSharp.Internal
{
    internal sealed class ObserverNode<T> : IObserver<T>, IDisposable
    {
        readonly IObserver<T> _observer;

        IObserverLinkedList<T>? _list;

        internal ObserverNode<T>? Previous
        {
            get;
            set;
        }

        internal ObserverNode<T>? Next
        {
            get;
            set;
        }

        /// <summary>Creates a subscription node in an observer list.</summary>
        /// <param name="list">The list that owns this subscription.</param>
        /// <param name="observer">The observer that receives forwarded notifications.</param>
        public ObserverNode(IObserverLinkedList<T> list, IObserver<T> observer)
        {
            _list = list;
            _observer = observer;
        }

        /// <summary>Forwards the completion notification to the observer.</summary>
        public void OnCompleted()
        {
            _observer.OnCompleted();
        }

        /// <summary>Forwards an error notification to the observer.</summary>
        /// <param name="error">The error reported by the provider.</param>
        public void OnError(Exception error)
        {
            _observer.OnError(error);
        }

        /// <summary>Forwards a state update to the observer.</summary>
        /// <param name="value">The state supplied to the observer.</param>
        public void OnNext(T value)
        {
            _observer.OnNext(value);
        }

        /// <summary>Removes this subscription from its observer list once.</summary>
        public void Dispose()
        {
            IObserverLinkedList<T>? sourceList = Interlocked.Exchange(ref _list, null);
            if (sourceList != null)
            {
                sourceList.UnsubscribeNode(this);
            }
        }
    }
}
