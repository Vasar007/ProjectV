using System;

namespace ThingAppraiser.Models.Internal
{
    public sealed class Rating : IEquatable<Rating>
    {
        public Guid RatingId { get; }

        public string RatingName { get; }


        public Rating(Guid ratingId, string ratingName)
        {
            RatingId = ratingId;
            RatingName = ratingName;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is Rating other)) return false;

            return IsEqual(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return RatingId.GetHashCode();
        }

        #endregion

        #region  IEquatable<Rating> Implementation

        /// <inheritdoc />
        public bool Equals(Rating? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="Rating" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(Rating? left, Rating? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="Rating" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(Rating? left, Rating? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="Rating" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        private bool IsEqual(Rating other)
        {
            return RatingId == other.RatingId &&
                   string.Equals(RatingName, other.RatingName, StringComparison.InvariantCulture);
        }
    }
}
