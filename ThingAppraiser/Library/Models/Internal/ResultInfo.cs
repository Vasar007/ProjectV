using System;

namespace ThingAppraiser.Models.Internal
{
    /// <summary>
    /// Represent result of internal calculations and data processing.
    /// </summary>
    public sealed class ResultInfo
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
        public void Deconstruct(out int thingId, out double ratingValue,
            out Guid ratingId)
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
    }
}
