using System;
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
        }

        public class AppReducer : IReducer<AppState>
        {
            public AppState Invoke(AppState state, IAction action)
            {
                return state ?? new AppState();
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
    }
}
