using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReduxSharp.Linq;
using Xunit;

namespace ReduxSharp.Tests
{
    public class ObservableTest
    {
        public class AppState
        {
            public int Count { get; set; }
        }

        public class IncrementAction { }

        public class ReplaceAction
        {
            public ReplaceAction(int count) => Count = count;

            public int Count { get; }
        }

        public class AppReducer : IReducer<AppState>
        {
            public async ValueTask<AppState> Invoke<TAction>(AppState state, TAction action)
            {
                state = state ?? new AppState() { Count = 0 };

                switch (action)
                {
                    case IncrementAction _:
                        return await Task.Run(() =>
                        {
                            return new AppState
                            {
                                Count = state.Count + 1,
                            };
                        });
                    case ReplaceAction a:
                        return new AppState
                        {
                            Count = a.Count,
                        };
                    default:
                        return state;
                }
            }
        }

        [Fact]
        public async Task SelectTest()
        {
            var values = new List<int>();
            var store = new Store<AppState>(new AppReducer(), new AppState());
            store.Select(s => s.Count).Subscribe(new ActionObserver<int>()
            {
                Next = (value) =>
                {
                    values.Add(value);
                }
            });

            await store.Dispatch(new IncrementAction());
            await store.Dispatch(new IncrementAction());
            await store.Dispatch(new IncrementAction());

            Assert.Equal(1, values[0]);
            Assert.Equal(2, values[1]);
            Assert.Equal(3, values[2]);
        }

        [Fact]
        public void Select_throws_ArgumentNullException_when_selector_is_null()
        {
            var store = new Store<AppState>(new AppReducer(), new AppState());
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.Select<AppState, int>(null);
            });
        }

        [Fact]
        public void Select_throws_ArgumentNullException_when_source_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Observable.Select<AppState, int>(null, s => s.Count);
            });
        }

        [Fact]
        public async Task DistinctUntilChangedTest()
        {
            var values = new List<int>();
            var store = new Store<AppState>(new AppReducer(), new AppState());
            store.Select(s => s.Count)
                .DistinctUntilChanged()
                .Subscribe(new ActionObserver<int>()
                {
                    Next = (value) =>
                    {
                        values.Add(value);
                    }
                });

            await store.Dispatch(new ReplaceAction(1));
            await store.Dispatch(new ReplaceAction(1));
            await store.Dispatch(new ReplaceAction(2));
            await store.Dispatch(new ReplaceAction(2));
            await store.Dispatch(new ReplaceAction(3));
            await store.Dispatch(new ReplaceAction(3));

            Assert.Equal(1, values[0]);
            Assert.Equal(2, values[1]);
            Assert.Equal(3, values[2]);
        }

        [Fact]
        public void DistinctUntilChanged_throws_ArgumentNullException_when_source_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Observable.DistinctUntilChanged<AppState>(null);
            });
            Assert.Throws<ArgumentNullException>(() =>
            {
                Observable.DistinctUntilChanged(null, EqualityComparer<AppState>.Default);
            });
        }

        [Fact]
        public void DistinctUntilChanged_throws_ArgumentNullException_when_comparer_is_null()
        {
            var store = new Store<AppState>(new AppReducer(), new AppState());
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.DistinctUntilChanged(null);
            });
        }
    }
}
