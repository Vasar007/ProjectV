using System;

namespace ThingAppraiser.Models.Internal
{
    /// <summary>
    /// Represent result of internal calculations and data processing.
    /// </summary>
    public sealed class ResultInfo : IEquatable<ResultInfo>
    {
        /// <summary>
        /// Thing ID.
        /// </summary>
        public int ThingId { get; }

        /// <summary>
        /// Calculated rating value of Thing.
        /// </summary>
        public double RatingValue { get; }

        /// <summary>
        /// Unique identifier of the rating.
        /// </summary>
        public Guid RatingId { get; }


        /// <summary>
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="thingId">ID of the Thing.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        /// <param name="ratingId">ID of the rating.</param>
        public ResultInfo(int thingId, double ratingValue, Guid ratingId)
        {
            ThingId = thingId;
            RatingValue = ratingValue;
            RatingId = ratingId;
        }

        /// <summary>
        /// Deconstruct object and get all field values.
        /// </summary>
        /// <param name="thingId">Variable to write The Thing ID.</param>
        /// <param name="ratingValue">Variable to write rating value.</param>
        /// <param name="ratingId">Variable to write rating ID.</param>
        /// <remarks>Need to work with deconstructing user-defined types.</remarks>
        public void Deconstruct(out int thingId, out double ratingValue, out Guid ratingId)
        {
            thingId = ThingId;
            ratingValue = RatingValue;
            ratingId = RatingId;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return RatingId + " " + ThingId + " " + RatingValue;
        }

        #endregion

        #region Object Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as ResultInfo);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(ThingId, RatingValue, RatingId);
        }

        #endregion

        #region IEquatable<BasicInfo> Implementation

        /// <inheritdoc />
        public bool Equals(ResultInfo? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="ResultInfo" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(ResultInfo? left, ResultInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="ResultInfo" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(ResultInfo? left, ResultInfo? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="ResultInfo" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        private bool IsEqual(ResultInfo other)
        {
            const double eps = 1e-6;
            return ThingId.Equals(other.ThingId) &&
                   Math.Abs(RatingValue - other.RatingValue) < eps &&
                   RatingId.Equals(other.RatingId);
        }
    }
}
