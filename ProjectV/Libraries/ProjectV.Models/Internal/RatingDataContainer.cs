using System;
using Newtonsoft.Json;
using ProjectV.Models.Data;

namespace ProjectV.Models.Internal
{
    /// <summary>
    /// Represent result of internal calculations and data processing.
    /// </summary>
    public sealed class RatingDataContainer : IEquatable<RatingDataContainer>
    {
        /// <summary>
        /// Contains all data about appraised Thing.
        /// </summary>
        public BasicInfo DataHandler { get; }

        /// <summary>
        /// Calculated rating value of Thing.
        /// </summary>
        public double RatingValue { get; }

        /// <summary>
        /// Rating identifier.
        /// </summary>
        public Guid RatingId { get; }


        /// <summary>
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="dataHandler">Data handler of the Thing.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        [JsonConstructor]
        public RatingDataContainer(
            BasicInfo dataHandler,
            double ratingValue,
            Guid ratingId)
        {
            DataHandler = dataHandler;
            RatingValue = ratingValue;
            RatingId = ratingId;
        }

        /// <summary>
        /// Deconstruct object and get all field values.
        /// </summary>
        /// <param name="dataHandler">Variable to write The Thing.</param>
        /// <param name="ratingValue">Variable to write rating value.</param>
        /// <remarks>Need to work with deconstructing user-defined types.</remarks>
        public void Deconstruct(
            out BasicInfo dataHandler,
            out double ratingValue,
            out Guid ratingId)
        {
            dataHandler = DataHandler;
            ratingValue = RatingValue;
            ratingId = RatingId;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{DataHandler.Title} {RatingValue.ToString()} (Rating ID: '{RatingId.ToString()}')";
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as RatingDataContainer);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(DataHandler, RatingValue, RatingId);
        }

        #endregion

        #region IEquatable<BasicInfo> Implementation

        /// <inheritdoc />
        public bool Equals(RatingDataContainer? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            return DataHandler.Equals(other.DataHandler) &&
                   RatingValue.Equals(other.RatingValue) &&
                   RatingId.Equals(other.RatingId);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="RatingDataContainer" /> are
        /// equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(RatingDataContainer? left, RatingDataContainer? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="RatingDataContainer" /> are not
        /// equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(RatingDataContainer? left, RatingDataContainer? right)
        {
            return !(left == right);
        }
    }
}
