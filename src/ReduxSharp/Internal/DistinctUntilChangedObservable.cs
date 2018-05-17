using System;
using System.Collections.Generic;
using System.Text;

namespace ReduxSharp.Internal
{
    internal class DistinctUntilChangedObservable<T> : IObservable<T>
    {
        readonly IObservable<T> source;

        readonly IEqualityComparer<T> comparer;

        public DistinctUntilChangedObservable(IObservable<T> source, IEqualityComparer<T> comparer)
        {
            this.source = source;
            this.comparer = comparer;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(new DistinctUntilChanged(this, observer));
        }

        class DistinctUntilChanged : IObserver<T>
        {
            readonly DistinctUntilChangedObservable<T> parent;

            readonly IObserver<T> observer;

            T lastValue = default(T);

            public DistinctUntilChanged(DistinctUntilChangedObservable<T> parent, IObserver<T> observer)
            {
                this.parent = parent;
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
                if (!parent.comparer.Equals(lastValue, value))
                {
                    lastValue = value;
                    observer.OnNext(value);
                }
            }
        }
    }
}
