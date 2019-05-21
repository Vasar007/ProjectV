using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Concrete appraiser for TMDb data.
    /// </summary>
    public class TmdbAppraiser : MoviesAppraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = "TmdbAppraiser";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on popularity and votes";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public TmdbAppraiser()
        {
        }

        protected static double CalculateRating(TmdbMovieInfo entity, 
            MinMaxDenominator voteCountMMD, MinMaxDenominator voteAverageMMD, 
            MinMaxDenominator popularityMMD)
        {
            double baseValue = CalculateRating(entity, voteCountMMD, voteAverageMMD);
            double popValue = (entity.Popularity - popularityMMD.MinValue) / 
                              popularityMMD.Denominator;

            return baseValue + popValue;
        }

        #region MoviesAppraiser Overriden Methods

        /// <inheritdoc />
        /// <remarks>
        /// Considers popularity value in addition to average vote and vote count.
        /// </remarks>
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
            if (!rawData.All(e => e is TmdbMovieInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeId.FullName}"
                );
            }

            MinMaxDenominator voteCountMMD = rawDataContainer.GetParameter("VoteCount");
            MinMaxDenominator voteAverageMMD = rawDataContainer.GetParameter("VoteAverage");
            MinMaxDenominator popularityMMD = rawDataContainer.GetParameter("Popularity");

            var converted = rawData.Select(e => (TmdbMovieInfo) e);
            foreach (TmdbMovieInfo entityInfo in converted)
            {
                double ratingValue = CalculateRating(entityInfo, voteCountMMD, voteAverageMMD,
                                                     popularityMMD);

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
