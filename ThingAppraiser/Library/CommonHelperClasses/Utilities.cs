using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser
{
    /// <summary>
    /// Adds extension methods for convenient.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Provides <c>null</c> check for every class.
        /// </summary>
        /// <typeparam name="T">Type which extenstion method will apply to.</typeparam>
        /// <param name="obj">Instance to check.</param>
        /// <param name="paramName">Name of the parameter for error message.</param>
        /// <returns>Returns passed value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> is <c>null</c>.
        /// </exception>
        public static T ThrowIfNull<T>(this T obj, string paramName)
            where T : class
        {
            if (obj is null)
            {
                throw new ArgumentNullException(paramName, $"{paramName} is null.");
            }
            return obj;
        }

        /// <summary>
        /// Checks if the string is <c>null</c> or empty.
        /// </summary>
        /// <param name="str">string to check.</param>
        /// <param name="paramName">Name of the parameter for error message.</param>
        /// <returns>The original string.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="str" /> is <c>null</c> or presents empty string.
        /// </exception>
        public static string ThrowIfNullOrEmpty(this string str, string paramName)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException($"{paramName} is null or empty.", paramName);
            }
            return str;
        }

        /// <summary>
        /// Checks if the string is <c>null</c>, empty or contains only whitespaces.
        /// </summary>
        /// <param name="str">string to check.</param>
        /// <param name="paramName">Name of the parameter for error message.</param>
        /// <returns>The original string.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="str" /> is <c>null</c>, presents empty string or contains only 
        /// whitespaces.
        /// </exception>
        public static string ThrowIfNullOrWhiteSpace(this string str, string paramName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException($"{paramName} is null, empty or contains only " +
                                            "whitespaces.", paramName);
            }
            return str;
        }

        /// <summary>
        /// Checks if enumerable is <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">Internal type of <see cref="IEnumerable{T}" />.</typeparam>
        /// <param name="collection">Enumerable to check.</param>
        /// <returns>
        /// Returns <c>true</c> in case the enumerable is <c>null</c> or empty, <c>false</c> 
        /// otherwise.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection is null || !collection.Any();
        }

        /// <summary>
        /// Compares string with invariant culture.
        /// </summary>
        /// <param name="str">First string to compare.</param>
        /// <param name="other">Second string to compare.</param>
        /// <returns><c>true</c> if strings are equal, <c>false</c> otherwise.</returns>
        public static bool IsEqualWithInvariantCulture(this string str, string other)
        {
            return string.Compare(str, other, StringComparison.InvariantCulture) == 0;
        }
    }
}
