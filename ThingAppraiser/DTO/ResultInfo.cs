using System;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Represent result of internal calculations and data processing.
    /// </summary>
    [Serializable]
    public class CResultInfo
    {
        /// <summary>
        /// Thing ID.
        /// </summary>
        public Int32 ThingID { get; }

        /// <summary>
        /// Calculated rating value of Thing.
        /// </summary>
        public Double RatingValue { get; }

        /// <summary>
        /// Unique identifier of the rating.
        /// </summary>
        public Guid RatingID { get; }


        /// <summary>
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="thingID">ID of the Thing.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        /// <param name="ratingID">ID of the rating.</param>
        public CResultInfo(Int32 thingID, Double ratingValue, Guid ratingID)
        {
            ThingID = thingID;
            RatingValue = ratingValue;
            RatingID = ratingID;
        }

        /// <summary>
        /// Deconstruct object and get all field values.
        /// </summary>
        /// <param name="thingID">Variable to write The Thing ID.</param>
        /// <param name="ratingValue">Variable to write rating value.</param>
        /// <param name="ratingID">Variable to write rating ID.</param>
        /// <remarks>Need to work with deconstructing user-defined types.</remarks>
        public void Deconstruct(out Int32 thingID, out Double ratingValue,
            out Guid ratingID)
        {
            thingID = ThingID;
            ratingValue = RatingValue;
            ratingID = RatingID;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override String ToString()
        {
            return RatingID + " " + ThingID + " " + RatingValue;
        }

        #endregion
    }
}
