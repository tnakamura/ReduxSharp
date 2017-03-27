using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ReduxSharp.Tests
{
    public class StoreTest
    {
        public class AppState
        {
            public int Count { get; set; }

            public class IncrementAction : IAction { }
        }

        public static class AppActionCreators
        {
            public static ActionCreator<AppState> Increment()
            {
                return (state, store) =>
                {
                    return new AppState.IncrementAction();
                };
            }

            public static AsyncActionCreator<AppState> IncrementTwice()
            {
                return async (state, store, callback) =>
                {
                    callback(Increment());
                    await Task.Delay(10);
                    callback(Increment());
                };
            }
        }

        public static class AppReducer
        {
            public static AppState Invoke(AppState state, IAction action)
            {
                state = state ?? new AppState() { Count = 0 };

                if (action is AppState.IncrementAction)
                {
                    state.Count += 1;
                }

                return state;
            }
        }

        [Fact]
        public void ConstructorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            Assert.NotNull(store);
        }

        [Fact]
        public void ConstructorNullReducerTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Store<AppState>(null);
            });
        }

        [Fact]
        public void DispatchNullActiontest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.Dispatch(default(IAction));
            });
        }

        [Fact]
        public void DispatchActionTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            store.Dispatch(new AppState.IncrementAction());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public void DispatchActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            store.Dispatch(AppActionCreators.Increment());
            Assert.Equal(1, store.State.Count);
        }

        [Fact]
        public async Task DispatchAsyncActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            await store.Dispatch(AppActionCreators.IncrementTwice());
            Assert.Equal(2, store.State.Count);
        }

        [Fact]
        public void DispatchNullActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            ActionCreator<AppState> actionCreator = null;
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.Dispatch(actionCreator);
            });
        }

        [Fact]
        public async Task DispatchNullAsyncActionCreatorTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke);
            AsyncActionCreator<AppState> asyncActionCreator = null;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await store.Dispatch(asyncActionCreator);
            });
        }

        [Fact]
        public async Task DispatchThreadSafeTest()
        {
            var store = new Store<AppState>(AppReducer.Invoke, new AppState());

            await Task.WhenAll(Enumerable.Range(0, 1000).Select(_ => Task.Run(() =>
              {
                  store.Dispatch(new AppState.IncrementAction());
              })));

            Assert.Equal(1000, store.State.Count);
        }
    }
}
