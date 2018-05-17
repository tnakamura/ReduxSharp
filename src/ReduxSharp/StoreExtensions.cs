using System;
using System.Collections.Generic;
using System.Text;
using ReduxSharp.Internal;

namespace ReduxSharp
{
    public static class StoreExtensions
    {
        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source)
        {
            return new DistinctUntilChangedObservable<T>(source, EqualityComparer<T>.Default);
        }

        public static IObservable<T> DistinctUntilChanged<T>(this IObservable<T> source, IEqualityComparer<T> comparer)
        {
            return new DistinctUntilChangedObservable<T>(source, comparer);
        }

        public static IObservable<TResult> Select<T, TResult>(this IObservable<T> source, Func<T, TResult> selector)
        {
            return new SelectObservable<T, TResult>(source, selector);
        }
    }
}
