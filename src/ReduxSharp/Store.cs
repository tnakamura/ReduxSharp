using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

namespace ReduxSharp
{
    public class Store<TState> : IStore<TState>
    {
        private readonly object _syncRoot = new object();

        private readonly IReducer<TState> _reducer;

        private readonly ListObserver<TState> _observer = new ListObserver<TState>();

        private readonly DispatchDelegate _dispatch;

        public Store(IReducer<TState> reducer, TState initialState = default(TState), params MiddlewareDelegate<TState>[] middlewares)
        {
            if (reducer == null) throw new ArgumentNullException(nameof(reducer));

            _reducer = reducer;
            _dispatch = ApplyMiddlewares(middlewares);

            if (initialState != null)
            {
                State = initialState;
            }
            else
            {
                _dispatch(new ReduxInitAction());
            }
        }

        public TState State { get; private set; }

        private DispatchDelegate ApplyMiddlewares(IEnumerable<MiddlewareDelegate<TState>> middlewares)
        {
            return middlewares
                .Reverse()
                .Aggregate<MiddlewareDelegate<TState>, DispatchDelegate>(
                    InternalDispatch,
                    (next, middleware) => middleware(this, next));
        }

        public IAction Dispatch(IAction action)
        {
            return _dispatch(action);
        }

        private IAction InternalDispatch(IAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            lock (_syncRoot)
            {
                var nextState = State = _reducer.Invoke(State, action);
                _observer.OnNext(nextState);
            }

            return action;
        }

        public IDisposable Subscribe(IObserver<TState> observer)
        {
            _observer.Add(observer);
            return new Subscription(this, observer);
        }

        private class Subscription : IDisposable
        {
            private readonly object _lockObj = new object();

            private Store<TState> _parent;

            private IObserver<TState> _target;

            public Subscription(Store<TState> parent, IObserver<TState> target)
            {
                _parent = parent;
                _target = target;
            }

            public void Dispose()
            {
                lock (_lockObj)
                {
                    lock (_parent._syncRoot)
                    {
                        _parent._observer.Remove(_target);
                    }
                    _target = null;
                    _parent = null;
                }
            }
        }
    }

    public delegate IAction DispatchDelegate(IAction action);

    public delegate DispatchDelegate MiddlewareDelegate<TState>(IStore<TState> store, DispatchDelegate next);
}
