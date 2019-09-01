using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ThingAppraiser
{
    /// <summary>
    /// A hash code used to help with implementing <see cref="object.GetHashCode()" />.
    /// </summary>
    /// <remarks>
    /// <see href="https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode" />
    /// <see href="https://rehansaeed.com/gethashcode-made-easy/" />
    /// </remarks>
    public struct HashCode : IEquatable<HashCode>
    {
        /// <summary>
        /// Default hash code value for empty collections.
        /// </summary>
        private const int EmptyCollectionPrimeNumber = 29;

        /// <summary>
        /// Field to accumulate hash code value.
        /// </summary>
        private readonly int value;

        /// <summary>
        /// Initializes a new instance of the <see cref="HashCode" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        private HashCode(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="HashCode" /> to <see cref="int" />.
        /// </summary>
        /// <param name="hashCode">The hash code.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.value;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(HashCode left, HashCode right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(HashCode left, HashCode right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Takes the hash code of the specified item.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>The new hash code.</returns>
        public static HashCode Of<T>(T item)
        {
            return new HashCode(GetHashCode(item));
        }

        /// <summary>
        /// Takes the hash code of the specified items.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The collection.</param>
        /// <returns>The new hash code.</returns>
        public static HashCode OfEach<T>(IEnumerable<T> items)
        {
            return items == null ? new HashCode(0) : new HashCode(GetHashCode(items, 0));
        }

        /// <summary>
        /// Adds the hash code of the specified item.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>The new hash code.</returns>
        public HashCode And<T>(T item)
        {
            return new HashCode(CombineHashCodes(value, GetHashCode(item)));
        }

        /// <summary>
        /// Adds the hash code of the specified items in the collection.
        /// </summary>
        /// <typeparam name="T">The type of the items.</typeparam>
        /// <param name="items">The collection.</param>
        /// <returns>The new hash code.</returns>
        public HashCode AndEach<T>(IEnumerable<T> items)
        {
            if (items == null)
            {
                return new HashCode(value);
            }

            return new HashCode(GetHashCode(items, value));
        }

        #region IEquatable<BasicInfo> Implementation

        /// <inheritdoc />
        public bool Equals(HashCode other)
        {
            return value.Equals(other.value);
        }

        #endregion

        #region Object Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is HashCode)
            {
                return Equals((HashCode) obj);
            }

            return false;
        }

        /// <summary>
        /// Throws <see cref="NotSupportedException" />.
        /// </summary>
        /// <returns>Does not return.</returns>
        /// <exception cref="NotSupportedException">Implicitly convert this struct to an
        /// <see cref="int" /> to get the hash code.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            throw new NotSupportedException(
                "Implicitly convert this struct to an int to get the hash code."
            );
        }

        #endregion

        /// <summary>
        /// Combines two hash code values.
        /// </summary>
        /// <param name="h1">The first hash code value.</param>
        /// <param name="h2">The second hash code value.</param>
        /// <returns>Combined result of specified hash code values.</returns>
        private static int CombineHashCodes(int h1, int h2)
        {
            unchecked
            {
                // Code copied from System.Tuple so it must be the best way to combine hash codes
                // or at least a good one.
                return ((h1 << 5) + h1) ^ h2;
            }
        }

        /// <summary>
        /// Null-safe invocation of GetHashCode method for <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of object to get hash code from.</typeparam>
        /// <param name="item">The value to get hash code from.</param>
        /// <returns>
        /// Returns result of item.GetHashCode method if item is not <c>null</c> or 0 otherwise.
        /// </returns>
        private static int GetHashCode<T>(T item)
        {
            return item?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Gets hash code for items in specified collection with start hash code value.
        /// </summary>
        /// <typeparam name="T">The type of items in collection.</typeparam>
        /// <param name="items">The collection to get hash code from.</param>
        /// <param name="startHashCode">An initial value for combining hash code.</param>
        /// <returns>
        /// Combined result of all items in collection. If collection is empty, default prime value
        /// woul be combined with specified <paramref name="startHashCode" />.
        /// </returns>
        private static int GetHashCode<T>(IEnumerable<T> items, int startHashCode)
        {
            int temp = startHashCode;

            IEnumerator<T> enumerator = items.GetEnumerator();
            if (enumerator.MoveNext())
            {
                temp = CombineHashCodes(temp, GetHashCode(enumerator.Current));

                while (enumerator.MoveNext())
                {
                    temp = CombineHashCodes(temp, GetHashCode(enumerator.Current));
                }
            }
            else
            {
                temp = CombineHashCodes(temp, EmptyCollectionPrimeNumber);
            }

            return temp;
        }
    }
}
