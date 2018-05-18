using ReduxSharp.Internal;
using System;
using System.Collections.Generic;

namespace ReduxSharp.Linq
{
    /// <summary>
    /// Provides a set of static methods for writing in-memory queries over observable
    /// sequences.
    /// </summary>
    public static class Observable
    {
        /// <summary>
        /// Returns an observable sequence that contains only distinct contiguous elements.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the source sequence.
        /// </typeparam>
        /// <param name="source">
        /// An observable sequence to retain distinct contiguous elements for.
        /// </param>
        /// <returns>
        /// An observable sequence only containing the distinct contiguous elements from
        /// the source sequence.
        /// </returns>
        public static IObservable<TSource> DistinctUntilChanged<TSource>(this IObservable<TSource> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source.DistinctUntilChanged(EqualityComparer<TSource>.Default);
        }

        /// <summary>
        /// Returns an observable sequence that contains only distinct contiguous elements
        /// according to the comparer.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the source sequence.
        /// </typeparam>
        /// <param name="source">
        /// An observable sequence to retain distinct contiguous elements for.
        /// </param>
        /// <param name="comparer">
        /// Equality comparer for source elements.
        /// </param>
        /// <returns>
        /// An observable sequence only containing the distinct contiguous elements from
        /// the source sequence.
        /// </returns>
        public static IObservable<TSource> DistinctUntilChanged<TSource>(this IObservable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            return new DistinctUntilChangedObservable<TSource>(source, comparer);
        }

        /// <summary>
        /// Projects each element of an observable sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the source sequence.
        /// </typeparam>
        /// <typeparam name="TResult">
        /// The type of the elements in the result sequence, obtained by running the selector
        /// function for each element in the source sequence.
        /// </typeparam>
        /// <param name="source">
        /// A sequence of elements to invoke a transform function on.
        /// </param>
        /// <param name="selector">
        /// A transform function to apply to each source element.
        /// </param>
        /// <returns>
        /// An observable sequence whose elements are the result of invoking the transform
        /// function on each element of source.
        /// </returns>
        public static IObservable<TResult> Select<TSource, TResult>(this IObservable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return new SelectObservable<TSource, TResult>(source, selector);
        }
    }
}
