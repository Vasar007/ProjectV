using Newtonsoft.Json;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Models.Internal
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
        /// Constructor which initializes fields.
        /// </summary>
        /// <param name="dataHandler">Data handler of the Thing.</param>
        /// <param name="ratingValue">Calculated rating value.</param>
        [JsonConstructor]
        public RatingDataContainer(BasicInfo dataHandler, double ratingValue)
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
        public void Deconstruct(out BasicInfo dataHandler, out double ratingValue)
        {
            dataHandler = DataHandler;
            ratingValue = RatingValue;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override string ToString()
        {
            return DataHandler.Title + " " + RatingValue;
        }

        #endregion
    }
}
