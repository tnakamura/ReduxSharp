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

        public class AppReducer : IReducer<AppState>
        {
            public AppState Invoke(AppState state, IAction action)
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
        public void Constructor_initialize_new_instance()
        {
            var store = new Store<AppState>(new AppReducer());
            Assert.NotNull(store);
        }

        [Fact]
        public void Constructor_throws_ArgumentNullException_when_reducer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new Store<AppState>(null);
            });
        }

        [Fact]
        public void Dispatch_throws_ArgumentNullException_when_action_is_null()
        {
            var store = new Store<AppState>(new AppReducer());
            Assert.Throws<ArgumentNullException>(() =>
            {
                store.Dispatch(null);
            });
        }

        [Fact]
        public async Task Dispatch_should_be_thread_safe()
        {
            var store = new Store<AppState>(new AppReducer(), new AppState());

            await Task.WhenAll(Enumerable.Range(0, 1000).Select(_ => Task.Run(() =>
              {
                  store.Dispatch(new AppState.IncrementAction());
              })));

            Assert.Equal(1000, store.State.Count);
        }
    }
}
