using System;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Represent result of internal calculations and data processing.
    /// </summary>
    [Serializable]
    public class CRatingDataContainer
    {
        /// <summary>
        /// Contains all data about appraised Thing.
        /// </summary>
        public CBasicInfo DataHandler { get; }

        /// <summary>
        /// Calculated rating value of Thing.
        /// </summary>
        public Double RatingValue { get; }


        /// <summary>
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="dataHandler">Data handler of the Thing.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        public CRatingDataContainer(CBasicInfo dataHandler, Double ratingValue)
        {
            DataHandler = dataHandler;
            RatingValue = ratingValue;
        }

        /// <summary>
        /// Deconstruct object and get all field values.
        /// </summary>
        /// <param name="dataHandler">Variable to write The Thing.</param>
        /// <param name="ratingValue">Variable to write rating value.</param>
        /// <remarks>Need to work with deconstructing user-defined types.</remarks>
        public void Deconstruct(out CBasicInfo dataHandler, out Double ratingValue)
        {
            dataHandler = DataHandler;
            ratingValue = RatingValue;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override String ToString()
        {
            return DataHandler.Title + " " + RatingValue;
        }

        #endregion
    }
}
