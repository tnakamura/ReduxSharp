using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ReduxSharp.Tests;

/// <summary>Verifies middleware ordering, caching, and exception propagation.</summary>
public sealed class MiddlewarePipelineTests
{
    readonly record struct Add(int Amount);
    sealed record Subtract(int Amount);

    sealed class Reducer : IReducer<int>
    {
        /// <inheritdoc/>
        public int Invoke<TAction>(int state, TAction action) => action switch
        {
            Add add => state + add.Amount,
            Subtract subtract => state - subtract.Amount,
            Exception error => throw error,
            _ => throw new ArgumentException("Unknown test action")
        };
    }

    sealed class Stage : IMiddlewareFactory<int>
    {
        readonly Action<IStore<int>, Action> _invoke;

        /// <summary>Counts the middleware pipeline builds.</summary>
        public int Builds;

        /// <summary>Creates a middleware stage with the supplied callback.</summary>
        public Stage(Action<IStore<int>, Action> invoke) => _invoke = invoke;

        /// <inheritdoc/>
        public Action<TAction> Build<TAction>(IStore<int> store, Action<TAction> next)
        {
            Interlocked.Increment(ref Builds);
            return action => _invoke(store, () => next(action));
        }
    }

    sealed class Observer : IObserver<int>
    {
        readonly Action<int> _notify;

        /// <summary>Creates an observer that forwards state updates to the callback.</summary>
        public Observer(Action<int> notify) => _notify = notify;

        /// <inheritdoc/>
        public void OnNext(int state) => _notify(state);

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => throw error;
    }

    /// <summary>Verifies repeated continuation calls and notification ordering.</summary>
    [Fact]
    public void NextCanRunTwiceAndNotificationsPrecedeUnwinding()
    {
        var log = new List<string>();
        Store<int> store = new StoreBuilder<int>(0)
            .Use(new Stage((_, next) =>
            {
                log.Add("outer");
                next();
                next();
                log.Add("end");
            }))
            .Use(new Stage((_, next) =>
            {
                log.Add("inner");
                next();
                log.Add("after");
            }))
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

    /// <summary>Verifies independent store pipelines and middleware registration snapshots.</summary>
    [Fact]
    public void EachBuildSnapshotsFactoriesAndOwnsItsTypedEntries()
    {
        var pass = new Stage((_, next) => next());
        IStoreBuilder<int> builder = new StoreBuilder<int>(0);
        Assert.Same(builder, builder.Use(pass));
        Store<int> first = builder.Build(new Reducer());
        Store<int> second = builder.Build(new Reducer());
        Store<int> stopped = builder.Use(new Stage((_, _) =>
        {
        })).Build(new Reducer());

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

    /// <summary>Verifies that nested dispatch starts at the first middleware stage.</summary>
    [Fact]
    public void DispatchOfAnotherTypeRestartsAtTheHead()
    {
        var visits = 0;
        Store<int> store = new StoreBuilder<int>(0).Use(new Stage((currentStore, next) =>
        {
            if (++visits == 1)
            {
                currentStore.Dispatch(new Subtract(1));
                Assert.Equal(-1, currentStore.State);
            }
            next();
        })).Build(new Reducer());
        store.Dispatch(new Add(3));
        Assert.Equal(2, visits);
        Assert.Equal(2, store.State);
    }

    /// <summary>Verifies that reducer failures propagate without publishing state.</summary>
    [Fact]
    public void ReducerExceptionPropagatesWithoutPublishingAnUpdate()
    {
        Store<int> store = new StoreBuilder<int>(0).Use(new Stage((_, next) => next())).Build(new Reducer());
        var states = new List<int>();
        using IDisposable subscription = store.Subscribe(new Observer(states.Add));
        var failure = new InvalidOperationException();
        Assert.Same(failure, Assert.Throws<InvalidOperationException>(() => store.Dispatch(failure)));
        store.Dispatch(new Add(1));
        Assert.Equal(new[] { 0, 1 }, states);
    }

    sealed class FailFirstBuild : IMiddlewareFactory<int>
    {
        /// <summary>Counts the middleware build attempts.</summary>
        public int Attempts;

        /// <inheritdoc/>
        public Action<TAction> Build<TAction>(IStore<int> store, Action<TAction> next)
        {
            if (++Attempts == 1)
            {
                throw new InvalidOperationException();
            }

            return next;
        }
    }

    /// <summary>Verifies that a failed pipeline build can be retried.</summary>
    [Fact]
    public void FailedBuildCanBeRetriedWithoutCachingAPartialChain()
    {
        var factory = new FailFirstBuild();
        Store<int> store = new StoreBuilder<int>(0).Use(factory).Build(new Reducer());
        Assert.Throws<InvalidOperationException>(() => store.Dispatch(new Add(1)));
        Assert.Equal(0, store.State);
        store.Dispatch(new Add(1));
        store.Dispatch(new Add(1));
        Assert.Equal(2, factory.Attempts);
        Assert.Equal(2, store.State);
    }

    /// <summary>Verifies pipeline caching and state updates under concurrent dispatch.</summary>
    [Fact]
    public void ConcurrentDispatchBuildsOneEntryAndSerializesUpdates()
    {
        var pass = new Stage((_, next) => next());
        Store<int> store = new StoreBuilder<int>(0).Use(pass).Build(new Reducer());
        Parallel.For(0, 1000, _ => store.Dispatch(new Add(1)));
        Assert.Equal(1, pass.Builds);
        Assert.Equal(1000, store.State);
    }
}
