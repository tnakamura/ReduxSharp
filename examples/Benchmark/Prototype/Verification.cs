using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReduxSharp;

namespace Benchmark.Prototype
{
    internal static class Verification
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

        sealed class Stage(Action<IStore<int>, Action> invoke) : IMiddlewareFactory<int>
        {
            public int Builds;
            public Action<TAction> Build<TAction>(IStore<int> store, Action<TAction> next)
            {
                Builds++;
                return action => invoke(store, () => next(action));
            }
        }

        sealed class Observer(Action<int> notify) : IObserver<int>
        {
            public void OnNext(int state) => notify(state);
            public void OnCompleted() { }
            public void OnError(Exception error) => throw error;
        }

        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
        }

        public static void Run()
        {
            var log = new List<string>();
            var twice = new Stage((_, next) =>
            {
                log.Add("outer");
                next();
                next();
                log.Add("end");
            });
            var inner = new Stage((_, next) =>
            {
                log.Add("inner");
                next();
                log.Add("after");
            });
            var builder = new StoreBuilder<int>(0).Use(twice).Use(inner);
            var store = builder.Run(new Reducer());
            using (store.Subscribe(new Observer(n => log.Add(n.ToString()))))
            {
                var add = store.For<Add>();
                Require(ReferenceEquals(add, store.For<Add>()), "Entry was rebuilt.");
                add(new Add(2));
                Require(log.SequenceEqual(new[] { "0", "outer", "inner", "2", "after", "inner", "4", "after", "end" }),
                    "Middleware or notification order changed.");
            }
            store.For<Subtract>()(new Subtract(1));
            Require(store.State == 2 && twice.Builds == 2 && inner.Builds == 2, "Action types did not share state.");
            Require(!log.Contains("3"), "Disposed observer received a notification.");

            var isolated = builder.Use(new Stage((_, _) => { })).Run(new Reducer());
            isolated.Dispatch(new Add(1));
            Require(isolated.State == 0 && store.State == 2, "Short circuit or store isolation failed.");
            store.Dispatch(new Add(1));
            Require(store.State == 4, "Builder edits affected an existing store.");

            bool nested = false;
            var nestedStore = new StoreBuilder<int>(0).Use(new Stage((s, next) =>
            {
                log.Add("enter");
                if (!nested)
                {
                    nested = true;
                    s.Dispatch(new Subtract(1));
                    Require(s.State == -1, "Nested dispatch did not complete synchronously.");
                }
                next();
            })).Run(new Reducer());
            log.Clear();
            nestedStore.Dispatch(new Add(3));
            Require(nestedStore.State == 2 && log.SequenceEqual(new[] { "enter", "enter" }), "Reentry failed.");

            var error = new InvalidOperationException("test");
            var seen = new List<int>();
            using var subscription = nestedStore.Subscribe(new Observer(seen.Add));
            try
            {
                nestedStore.Dispatch(error);
                throw new Exception("Missing reducer exception.");
            }
            catch (InvalidOperationException ex) when (ReferenceEquals(ex, error)) { }
            nestedStore.Dispatch(new Add(1));
            Require(seen.SequenceEqual(new[] { 2, 3 }), "Failed dispatch changed state or notified.");

            var pass = new Stage((_, next) => next());
            var concurrent = new StoreBuilder<int>(0).Use(pass).Run(new Reducer());
            var entries = new ConcurrentBag<Action<Add>>();
            Parallel.For(0, 1000, _ =>
            {
                var entry = concurrent.For<Add>();
                entries.Add(entry);
                entry(new Add(1));
            });
            Require(entries.All(e => ReferenceEquals(e, concurrent.For<Add>())) &&
                pass.Builds == 1 && concurrent.State == 1000, "Concurrent build or dispatch failed.");
            Console.WriteLine("Typed middleware behavior checks passed.");
        }
    }
}
