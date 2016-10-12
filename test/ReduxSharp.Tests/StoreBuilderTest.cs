using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReduxSharp.Tests
{
    public class StoreBuilderTest
    {
        public class AppState { }

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
            var builder = new StoreBuilder<AppState>(new AppReducer());
            Assert.NotNull(builder);
        }

        [Fact]
        public void Constructor_throws_ArgumentNullException_when_reducer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new StoreBuilder<AppState>(null);
            });
        }

        [Fact]
        public void Build_returns_new_store_instance()
        {
            var builder = new StoreBuilder<AppState>(new AppReducer());
            var store = builder.Build();
            Assert.NotNull(store);
        }
    }
}
