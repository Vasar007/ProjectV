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
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the zero-based index of the first occurrence within the entire 
        /// <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of an element that matches the conditions 
        /// defined by <paramref name="predicate"/>, if found; otherwise it'll throw exception.
        /// </returns>
        public static int FindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            int foundIndex = source
                .Select((value, index) => (Value: value, Index: index))
                .Where(x => predicate(x.Value))
                .Select(x => x.Index)
                .First();
            return foundIndex;
        }
    }
}
