using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Concrete appraiser for Steam data.
    /// </summary>
    public class SteamAppraiser : GameAppraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = "SteamAppraiser";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(SteamGameInfo);

        /// <inheritdoc />
        public override string RatingName { get; } =
            "Rating based on subtraction of final and initial price";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public SteamAppraiser()
        {
        }

        #region GameAppraiser Overriden Methods

        /// <summary>
        /// Calculates rating which depends on final and initial price.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawDataContainer" /> contains instances of invalid type for this 
        /// appraiser.
        /// </exception>
        public override ResultList GetRatings(RawDataContainer rawDataContainer, bool outputResults)
        {
            CheckRatingId();

            var ratings = new ResultList();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.GetData();
            if (rawData.IsNullOrEmpty()) return ratings;

            // Check if list have proper type.
            if (!rawData.All(e => e is SteamGameInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeId.FullName}"
                );
            }

            var converted = rawData.Select(e => (SteamGameInfo) e);
            foreach (SteamGameInfo entityInfo in converted)
            {
                double ratingValue = decimal.ToDouble(entityInfo.Price) - entityInfo.VoteAverage;

                var resultInfo = new ResultInfo(entityInfo.ThingId, ratingValue, RatingId);
                ratings.Add(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        #endregion
    }
}
