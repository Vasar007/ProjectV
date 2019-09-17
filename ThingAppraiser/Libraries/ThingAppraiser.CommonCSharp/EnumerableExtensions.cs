using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser
{
    /// <summary>
    /// Contains useful methods to work with enumerable items.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns the first element of a sequence, or a specified default value if the sequence
        /// contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}" /> to return the first element of or default.
        /// </param>
        /// <param name="defaultValue">
        /// The specified value to return if the sequence contains no elements.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> is <c>null</c>.
        /// </exception>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source,
            TSource defaultValue)
        {
            source.ThrowIfNull(nameof(source));

            foreach (TSource item in source)
            {
                return item;
            }

            return defaultValue;
        }

        /// <summary>
        /// Returns the first element of a sequence that satisfies the condition, or a specified 
        /// default value if the sequence contains no elements or no elements satisfy the condition.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}" /> to return the first element of or default.
        /// </param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="defaultValue">
        /// The specified value to return if the sequence contains no elements that satisfies the 
        /// condition.
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="predicate" /> is <c>null</c>.
        /// </exception>
        public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, TSource defaultValue)
        {
            source.ThrowIfNull(nameof(source));
            predicate.ThrowIfNull(nameof(predicate));

            foreach (TSource item in source)
            {
                if (predicate(item)) return item;
            }

            return defaultValue;
        }

        /// <summary>
        /// Searches for an element that satisfies the condition and returns the zero-based index of
        /// the first occurrence within the entire sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> to find element index.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that satisfies 
        /// <paramref name="predicate"/>, if found; otherwise it'll return -1.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="predicate" /> is <c>null</c>.
        /// </exception>
        public static int IndexOf<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate)
        {
            source.ThrowIfNull(nameof(source));
            predicate.ThrowIfNull(nameof(predicate));

            int foundIndex = source
                .Select((value, index) => (value, index))
                .Where(x => predicate(x.value))
                .Select(x => x.index)
                .FirstOrDefault(-1);

            return foundIndex;
        }

        /// <summary>
        /// Creates a <see cref="IReadOnlyDictionary{TKey, TValue}" /> from an
        /// <see cref="IEnumerable{T}" /> according to specified key selector and element selector
        /// functions.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// The type of the key returned by <paramref name="keySelector" />.
        /// </typeparam>
        /// <typeparam name="TElement">
        /// The type of the value returned by <paramref name="elementSelector" />
        /// .</typeparam>
        /// <param name="source">
        /// An <see cref="IEnumerable{T}" /> to create
        /// <see cref="IReadOnlyDictionary{TKey, TValue}" /> from.
        /// </param>
        /// <param name="keySelector">A function to extract a key from each element.</param>
        /// <param name="elementSelector">
        /// A transform function to produce a result element value from each element.
        /// </param>
        /// <returns>
        /// A <see cref="IReadOnlyDictionary{TKey, TValue}" /> that contains values of type
        /// <typeparamref name="TElement" /> selected from the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="keySelector" /> or
        /// <paramref name="elementSelector" /> is <c>null</c>. -or-
        /// <paramref name="keySelector" /> produces a key that is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="keySelector" /> produces duplicate keys for two elements.
        /// </exception>
        public static IReadOnlyDictionary<TKey, TElement>
            ToReadOnlyDictionary<TSource, TKey, TElement>(
                this IEnumerable<TSource> source,
                Func<TSource, TKey> keySelector,
                Func<TSource, TElement> elementSelector)
            where TKey: notnull
        {
            return source.ToDictionary(keySelector, elementSelector);
        }

        /// <summary>
        /// Creates a <see cref="IReadOnlyList{T}" /> from an <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}" /> to create a <see cref="IReadOnlyList{T}" /> from.
        /// </param>
        /// <returns>
        /// A <see cref="IReadOnlyList{T}" /> that contains elements from the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> is <c>null</c>.
        /// </exception>
        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.ToList();
        }

        /// <summary>
        /// Creates a <see cref="IReadOnlyCollection{T}" /> from an <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements of <paramref name="source" />.
        /// </typeparam>
        /// <param name="source">
        /// The <see cref="IEnumerable{T}" /> to create a <see cref="IReadOnlyCollection{T}" />
        /// from.
        /// </param>
        /// <returns>
        /// A <see cref="IReadOnlyCollection{T}" /> that contains elements from the input sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source" /> is <c>null</c>.
        /// </exception>
        public static IReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(
            this IEnumerable<TSource> source)
        {
            return source.ToList();
        }
    }
}
