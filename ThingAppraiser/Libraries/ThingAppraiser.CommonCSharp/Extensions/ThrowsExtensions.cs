using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.Extensions
{
    public static class ThrowsExtensions
    {
        /// <summary>
        /// Provides <c>null</c> check for every class.
        /// </summary>
        /// <typeparam name="T">Type which extenstion method will apply to.</typeparam>
        /// <param name="obj">Instance to check.</param>
        /// <param name="paramName">
        /// Name of the parameter for error message.
        /// Use operator <c>nameof</c> to get proper parameter name.
        /// </param>
        /// <returns>Returns passed value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="obj" /> is <c>null</c>. -or-
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        public static T ThrowIfNull<T>(this T obj, string paramName)
            where T : class?
        {
            if (paramName is null)
            {
                throw new ArgumentNullException(nameof(paramName));
            }

            if (obj is null)
            {
                throw new ArgumentNullException(paramName);
            }
            return obj;
        }

        /// <summary>
        /// Checks if the string is <c>null</c> or empty.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <param name="paramName">
        /// Name of the parameter for error message.
        /// Use operator <c>nameof</c> to get proper parameter name.
        /// </param>
        /// <returns>The original string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="str" /> is <c>null</c>. -or-
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="str" /> presents empty string.
        /// </exception>
        public static string ThrowIfNullOrEmpty(this string? str, string paramName)
        {
            paramName.ThrowIfNull(nameof(paramName));

            if (str is null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (str.Length == 0)
            {
                throw new ArgumentException($"{paramName} presents empty string.", paramName);
            }

            return str;
        }

        /// <summary>
        /// Checks if the string is <c>null</c>, empty or contains only whitespaces.
        /// </summary>
        /// <param name="str">String to check.</param>
        /// <param name="paramName">
        /// Name of the parameter for error message.
        /// Use operator <c>nameof</c> to get proper parameter name.
        /// </param>
        /// <returns>The original string.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="str" /> is <c>null</c>. -or-
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="str" /> presents empty string or contains only whitespaces.
        /// </exception>
        public static string ThrowIfNullOrWhiteSpace(this string? str, string paramName)
        {
            paramName.ThrowIfNull(nameof(paramName));

            if (str is null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (str.Length == 0)
            {
                throw new ArgumentException($"{paramName} presents empty string.", paramName);
            }
            if (str.All(char.IsWhiteSpace))
            {
                throw new ArgumentException($"{paramName} contains only whitespaces.", paramName);
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="collection" /> is <c>null</c>. -or-
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="collection" /> contains no elements.
        /// </exception>
        public static IEnumerable<T> ThrowIfNullOrEmpty<T>(this IEnumerable<T>? collection,
            string paramName)
        {
            paramName.ThrowIfNull(nameof(paramName));

            if (collection is null)
            {
                throw new ArgumentNullException(paramName);
            }
            if (!collection.Any())
            {
                throw new ArgumentException($"{paramName} contains no elements.", paramName);
            }

            return collection;
        }

        /// <summary>
        /// Checks if the guid is empty.
        /// </summary>
        /// <param name="guid">Guid to check.</param>
        /// <param name="paramName">
        /// Name of the parameter for error message.
        /// Use operator <c>nameof</c> to get proper parameter name.
        /// </param>
        /// <returns>The original guid.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="paramName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="guid" /> presents empty guid.
        /// </exception>
        public static Guid ThrowIfEmpty(this Guid guid, string paramName)
        {
            paramName.ThrowIfNull(nameof(paramName));

            if (guid.IsEmpty())
            {
                throw new ArgumentException($"{paramName} is empty.", paramName);
            }

            return guid;
        }
    }
}
