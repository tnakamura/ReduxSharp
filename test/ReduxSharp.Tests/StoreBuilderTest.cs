using System;
using Xunit;

namespace ReduxSharp.Tests
{
    public class StoreBuilderTest
    {
        public class AppState { }

        public static class AppReducer
        {
            public static AppState Invoke(AppState state, IAction action)
            {
                return state ?? new AppState();
            }
        }

        [Fact]
        public void Constructor_initialize_new_instance()
        {
            var builder = new StoreBuilder<AppState>(AppReducer.Invoke);
            Assert.NotNull(builder);
        }

        [Fact]
        public void Constructor_throws_ArgumentNullException_when_reducer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new StoreBuilder<AppState>(null as IReducer<AppState>);
            });
        }

        [Fact]
        public void Build_returns_new_store_instance()
        {
            var builder = new StoreBuilder<AppState>(AppReducer.Invoke);
            var store = builder.Build();
            Assert.NotNull(store);
        }

        [Fact]
        public void UseMiddleware_add_class_type_middleware_to_store()
        {
            var options = new LoggerOptions();
            var store = new StoreBuilder<AppState>(AppReducer.Invoke)
                .UseLogger(options)
                .Build();
            Assert.Single(options.Buffer);
            Assert.Equal(typeof(ReduxInitialAction).FullName, options.Buffer[0]);
        }

        [Fact]
        public void UseMiddleware_add_middleware_that_no_options()
        {
            var options = new LoggerOptions();
            var store = new StoreBuilder<AppState>(AppReducer.Invoke)
                .UseDummy()
                .UseLogger(options)
                .Build();
            Assert.Single(options.Buffer);
            Assert.Equal(typeof(DummyAction).FullName, options.Buffer[0]);
        }
    }
}
