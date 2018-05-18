using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace ReduxSharp.Tests
{
    public class StoreExtensionsTest
    {
        public class AppState
        {
            public int Count { get; set; }
        }

        public class IncrementAction : IAction { }

        public class ReplaceAction : IAction
        {
            public ReplaceAction(int count) => Count = count;

            public int Count { get; }
        }

        public static class AppReducer
        {
            public static AppState Invoke(AppState state, IAction action)
            {
                state = state ?? new AppState() { Count = 0 };

                switch (action)
                {
                    case IncrementAction _:
                        state.Count += 1;
                        break;
                    case ReplaceAction a:
                        state.Count = a.Count;
                        break;
                }

                return state;
            }
        }

        [Fact]
        public void SelectTest()
        {
            var values = new List<int>();
            var store = new Store<AppState>(AppReducer.Invoke);
            store.Select(s => s.Count).Subscribe(new ActionObserver<int>()
            {
                Next = (value) =>
                {
                    values.Add(value);
                }
            });

            store.Dispatch(new IncrementAction());
            store.Dispatch(new IncrementAction());
            store.Dispatch(new IncrementAction());

            Assert.Equal(1, values[0]);
            Assert.Equal(2, values[1]);
            Assert.Equal(3, values[2]);
        }

        [Fact]
        public void DistinctUntilChangedTest()
        {
            var values = new List<int>();
            var store = new Store<AppState>(AppReducer.Invoke);
            store.Select(s => s.Count)
                .DistinctUntilChanged()
                .Subscribe(new ActionObserver<int>()
                {
                    Next = (value) =>
                    {
                        values.Add(value);
                    }
                });

            store.Dispatch(new ReplaceAction(1));
            store.Dispatch(new ReplaceAction(1));
            store.Dispatch(new ReplaceAction(2));
            store.Dispatch(new ReplaceAction(2));
            store.Dispatch(new ReplaceAction(3));
            store.Dispatch(new ReplaceAction(3));

            Assert.Equal(1, values[0]);
            Assert.Equal(2, values[1]);
            Assert.Equal(3, values[2]);
        }
    }
}
