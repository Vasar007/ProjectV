using System;

namespace ThingAppraiser.Extensions
{
    /// <summary>
    /// Adds extension methods for convenient.
    /// </summary>
    public static class Utilities
    {
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
