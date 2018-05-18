using System;

namespace ReduxSharp
{
    public partial class Store<TState> : IStore<TState>
    {
        class Subscription : IDisposable
        {
            readonly object lockObj = new object();

            Store<TState> parent;

            IObserver<TState> target;

            public Subscription(Store<TState> parent, IObserver<TState> target)
            {
                this.parent = parent;
                this.target = target;
            }

            public void Dispose()
            {
                lock (lockObj)
                {
                    if (parent != null)
                    {
                        lock (parent.syncRoot)
                        {
                            parent.observer.Remove(target);
                        }
                    }
                    target = null;
                    parent = null;
                }
            }
        }
    }
}
