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
        /// Checks if enumerable is <c>null</c> or empty without throwing exception.
        /// </summary>
        /// <typeparam name="T">Internal type of <see cref="IEnumerable{T}" />.</typeparam>
        /// <param name="collection">Enumerable to check.</param>
        /// <returns>
        /// Returns <c>true</c> in case the enumerable is <c>null</c> or empty, <c>false</c> 
        /// otherwise.
        /// </returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
        {
            return collection is null || !collection.Any();
        }

        /// <summary>
        /// Compare guid with the empty guid without throwing exception.
        /// </summary>
        /// <param name="guid">Guid to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="guid" /> equals to empty guid, <c>falst</c> otherwise.
        /// </returns>
        public static bool IsEmpty(this Guid guid)
        {
            return guid == Guid.Empty;
        }

        /// <summary>
        /// Checks that <paramref name="potentialDescendant" /> is same type as
        /// <paramref name="potentialDescendant" /> or is the it subclass.
        /// </summary>
        /// <param name="potentialDescendant">Potential descendant type to check.</param>
        /// <param name="potentialBase">Potential base type to compare.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="potentialDescendant" /> is same type as
        /// <paramref name="potentialDescendant" /> or is the it subclass, <c>false</c> otherwise.
        /// </returns>
        public static bool IsSameOrSubclass(this Type potentialDescendant, Type potentialBase)
        {
            return potentialDescendant.IsSubclassOf(potentialBase) ||
                   potentialDescendant.Equals(potentialBase);
        }
    }
}
