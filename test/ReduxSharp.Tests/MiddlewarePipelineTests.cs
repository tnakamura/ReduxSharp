using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReduxSharp.Tests
{
    public sealed class MiddlewarePipelineTests
    {
        readonly record struct Add(int Amount);
        sealed record Subtract(int Amount);

        sealed class Reducer : IReducer<int>
        {
            public int Invoke<TAction>(int state, TAction action) => action switch
            {
                Add add => state + add.Amount,
                Subtract sub => state - sub.Amount,
                Exception error => throw error,
                _ => throw new ArgumentException("Unknown test action")
            };
        }

        sealed class Stage : IMiddlewareFactory<int>
        {
            readonly Action<IStore<int>, Action> invoke;
            public int Builds;

            public Stage(Action<IStore<int>, Action> invoke) => this.invoke = invoke;

            public Action<TAction> Build<TAction>(IStore<int> store, Action<TAction> next)
            {
                Interlocked.Increment(ref Builds);
                return action => invoke(store, () => next(action));
            }
        }

        sealed class Observer : IObserver<int>
        {
            readonly Action<int> notify;
            public Observer(Action<int> notify) => this.notify = notify;
            public void OnNext(int state) => notify(state);
            public void OnCompleted() { }
            public void OnError(Exception error) => throw error;
        }

        [Fact]
        public void NextCanRunTwiceAndNotificationsPrecedeUnwinding()
        {
            var log = new List<string>();
            var store = new StoreBuilder<int>(0)
                .Use(new Stage((_, next) => { log.Add("outer"); next(); next(); log.Add("end"); }))
                .Use(new Stage((_, next) => { log.Add("inner"); next(); log.Add("after"); }))
                .Build(new Reducer());
            using (store.Subscribe(new Observer(state => log.Add(state.ToString()))))
            {
                store.Dispatch(new Add(2));
            }
            Assert.Equal(new[] { "0", "outer", "inner", "2", "after", "inner", "4", "after", "end" }, log);
            log.Clear();
            store.Dispatch(new Add(1));
            Assert.DoesNotContain("5", log);
            Assert.DoesNotContain("6", log);
        }

        [Fact]
        public void EachBuildSnapshotsFactoriesAndOwnsItsTypedEntries()
        {
            var pass = new Stage((_, next) => next());
            IStoreBuilder<int> builder = new StoreBuilder<int>(0);
            Assert.Same(builder, builder.Use(pass));
            var first = builder.Build(new Reducer());
            var second = builder.Build(new Reducer());
            var stopped = builder.Use(new Stage((_, _) => { })).Build(new Reducer());

            first.Dispatch(new Add(4));
            second.Dispatch(new Add(3));
            stopped.Dispatch(new Add(10));
            first.Dispatch(new Subtract(1));
            first.Dispatch(new Add(2));
            Assert.Equal(4, pass.Builds);
            Assert.Equal(5, first.State);
            Assert.Equal(3, second.State);
            Assert.Equal(0, stopped.State);
        }

        [Fact]
        public void DispatchOfAnotherTypeRestartsAtTheHead()
        {
            int visits = 0;
            var store = new StoreBuilder<int>(0).Use(new Stage((s, next) =>
            {
                if (++visits == 1)
                {
                    s.Dispatch(new Subtract(1));
                    Assert.Equal(-1, s.State);
                }
                next();
            })).Build(new Reducer());
            store.Dispatch(new Add(3));
            Assert.Equal(2, visits);
            Assert.Equal(2, store.State);
        }

        [Fact]
        public void ReducerExceptionPropagatesWithoutPublishingAnUpdate()
        {
            var store = new StoreBuilder<int>(0).Use(new Stage((_, next) => next())).Build(new Reducer());
            var states = new List<int>();
            using var subscription = store.Subscribe(new Observer(states.Add));
            var failure = new InvalidOperationException();
            Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => store.Dispatch(failure)));
            store.Dispatch(new Add(1));
            Assert.Equal(new[] { 0, 1 }, states);
        }

        sealed class FailFirstBuild : IMiddlewareFactory<int>
        {
            public int Attempts;
            public Action<TAction> Build<TAction>(IStore<int> store, Action<TAction> next)
            {
                if (++Attempts == 1) throw new InvalidOperationException();
                return next;
            }
        }

        [Fact]
        public void FailedBuildCanBeRetriedWithoutCachingAPartialChain()
        {
            var factory = new FailFirstBuild();
            var store = new StoreBuilder<int>(0).Use(factory).Build(new Reducer());
            Assert.Throws<InvalidOperationException>(() => store.Dispatch(new Add(1)));
            Assert.Equal(0, store.State);
            store.Dispatch(new Add(1));
            store.Dispatch(new Add(1));
            Assert.Equal(2, factory.Attempts);
            Assert.Equal(2, store.State);
        }

        [Fact]
        public void ConcurrentDispatchBuildsOneEntryAndSerializesUpdates()
        {
            var pass = new Stage((_, next) => next());
            var store = new StoreBuilder<int>(0).Use(pass).Build(new Reducer());
            Parallel.For(0, 1000, _ => store.Dispatch(new Add(1)));
            Assert.Equal(1, pass.Builds);
            Assert.Equal(1000, store.State);
        }
    }
}
