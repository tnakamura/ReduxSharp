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

        [Fact]
        public void UseMiddleware_add_class_type_middleware_to_store()
        {
            var options = new LoggerOptions();
            var store = new StoreBuilder<AppState>(new AppReducer())
                .UseLogger(options)
                .Build();
            Assert.Equal(1, options.Buffer.Count);
            Assert.Equal(typeof(ReduxInitAction).FullName, options.Buffer[0]);
        }

        [Fact]
        public void UseMiddleware_add_middleware_that_no_options()
        {
            var options = new LoggerOptions();
            var store = new StoreBuilder<AppState>(new AppReducer())
                .UseDummy()
                .UseLogger(options)
                .Build();
            Assert.Equal(1, options.Buffer.Count);
            Assert.Equal(typeof(DummyAction).FullName, options.Buffer[0]);
        }
    }
}
