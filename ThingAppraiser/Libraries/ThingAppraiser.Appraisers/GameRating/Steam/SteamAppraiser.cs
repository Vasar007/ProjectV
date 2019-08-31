using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.GameRating.Steam
{
    /// <summary>
    /// Concrete appraiser for Steam data.
    /// </summary>
    public sealed class SteamAppraiser : GameAppraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(SteamAppraiser);

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
        public override IReadOnlyList<ResultInfo> GetRatings(RawDataContainer rawDataContainer,
            bool outputResults)
        {
            CheckRatingId();

            var ratings = new List<ResultInfo>();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.RawData;
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
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}");
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        #endregion
    }
}
