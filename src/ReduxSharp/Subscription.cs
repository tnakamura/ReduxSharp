using System;

namespace ReduxSharp
{
    public partial class Store<TState> : IStore<TState>
    {
        class Subscription : IDisposable
        {
            readonly object _lockObj = new object();

            Store<TState> _parent;

            IObserver<TState> _target;

            public Subscription(Store<TState> parent, IObserver<TState> target)
            {
                _parent = parent;
                _target = target;
            }

            public void Dispose()
            {
                lock (_lockObj)
                {
                    if (_parent != null)
                    {
                        lock (_parent._syncRoot)
                        {
                            _parent._observer.Remove(_target);
                        }
                    }
                    _target = null;
                    _parent = null;
                }
            }
        }
    }
}
