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
        /// Contains all data about appraised Thing.
        /// </summary>
        public CBasicInfo DataHandler { get; }

        /// <summary>
        /// Calculated rating value of Thing.
        /// </summary>
        public Single RatingValue { get; }


        /// <summary>
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="dataHandler">Instance of data class.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        public CResultInfo(CBasicInfo dataHandler, Single ratingValue)
        {
            DataHandler = dataHandler;
            RatingValue = ratingValue;
        }

        /// <summary>
        /// Deconstruct object and get all field values.
        /// </summary>
        /// <param name="dataHandler">Variable to write data about The Thing.</param>
        /// <param name="ratingValue">Variable to write rating value.</param>
        /// <remarks>Need to work with deconstructing user-defined types.</remarks>
        public void Deconstruct(out CBasicInfo dataHandler, out Single ratingValue)
        {
            dataHandler = DataHandler;
            ratingValue = RatingValue;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override String ToString()
        {
            return DataHandler.Title + RatingValue;
        }

        #endregion
    }
}
