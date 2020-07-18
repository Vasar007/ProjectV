using System;
using Newtonsoft.Json;
using ProjectV.Models.Data;

namespace ProjectV.Models.Internal
{
    /// <summary>
    /// Represent result of internal calculations and data processing.
    /// </summary>
    public sealed class RatingDataContainer
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
        public Guid RatingId { get; set; }


        /// <summary>
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="dataHandler">Data handler of the Thing.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        [JsonConstructor]
        public RatingDataContainer(BasicInfo dataHandler, double ratingValue, Guid ratingId)
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
        public void Deconstruct(out BasicInfo dataHandler, out double ratingValue, out Guid ratingId)
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

        #endregion
    }
}
