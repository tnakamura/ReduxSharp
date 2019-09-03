using System;

namespace ReduxSharp.Linq
{
    internal sealed class SelectObservable<T, TResult> : IObservable<TResult>
    {
        readonly IObservable<T> source;

        readonly Func<T, TResult> selector;

        public SelectObservable(IObservable<T> source, Func<T, TResult> selector)
        {
            this.source = source;
            this.selector = selector;
        }

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            return source.Subscribe(new Select(this, observer));
        }

        sealed class Select : IObserver<T>
        {
            readonly SelectObservable<T, TResult> parent;

            readonly IObserver<TResult> observer;

            public Select(SelectObservable<T, TResult> parent, IObserver<TResult> observer)
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
                observer.OnNext(parent.selector(value));
            }
        }
    }
}
