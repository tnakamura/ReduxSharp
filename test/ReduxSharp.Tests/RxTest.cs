using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Reactive;
using System.Reactive.Linq;

namespace ReduxSharp.Tests
{
    public class RxTest
    {
        public class AppState
        {
            public int Count { get; set; } = 0;
        }

        public class IncrementAction { }

        public class RaiseExceptionAction
        {
            public Exception Exception { get; }

            public RaiseExceptionAction(Exception exception) => Exception = exception;
        }

        public class AppReducer : IReducer<AppState>
        {
            public AppState Invoke<TAction>(AppState state, in TAction action)
            {
                state = state ?? new AppState();
                switch (action)
                {
                    case IncrementAction _:
                        return new AppState
                        {
                            Count = state.Count + 1,
                        };
                    case RaiseExceptionAction e:
                        throw e.Exception;
                    default:
                        return state;
                }
            }
        }

        [Fact]
        public void OnErrorTest()
        {
            var store = new Store<AppState>(new AppReducer(), new AppState());

            var states = new List<AppState>();
            var errors = new List<Exception>();
            var completed = false;

            store.Subscribe(
                onNext: x =>
                {
                    states.Add(x);
                },
                onError: x =>
                {
                    errors.Add(x);
                },
                onCompleted: () =>
                {
                    completed = true;
                });

            store.Dispatch(
                new IncrementAction());
            store.Dispatch(
                new RaiseExceptionAction(
                    new NotSupportedException()));

            Assert.Equal(1, store.State.Count);
            Assert.False(completed);

            Assert.Single(errors);
            Assert.IsType<NotSupportedException>(errors[0]);

            Assert.Single(states);
            Assert.Equal(1, states[0].Count);
        }
    }
}
