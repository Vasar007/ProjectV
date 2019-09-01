using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Appraisers.Appraisals;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.MoviesRating.Tmdb
{
    /// <summary>
    /// Concrete appraiser for TMDb data.
    /// </summary>
    public sealed class TmdbAppraiser : MoviesAppraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(TmdbAppraiser);

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

        #region MoviesAppraiser Overriden Methods

        /// <inheritdoc />
        /// <remarks>
        /// Considers popularity value in addition to average vote and vote count.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawDataContainer" /> contains instances of invalid type for this 
        /// appraiser.
        /// </exception>
        public override IReadOnlyList<ResultInfo> GetRatings(RawDataContainer rawDataContainer,
            bool outputResults)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            CheckRatingId();

            var ratings = new List<ResultInfo>();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.RawData;
            if (!rawData.Any()) return ratings;

            // Check if list have proper type.
            if (!rawData.All(e => e is TmdbMovieInfo))
            {
                throw new ArgumentException(
                    $"One of element type is invalid for appraiser with type {TypeId.FullName}."
                );
            }

            // TODO: somehow inject this appraisals from the outside.
            var basicAppraisal = new BasicAppraisal(rawDataContainer);
            var appraisal = new TmdbAppraisal(basicAppraisal, rawDataContainer);

            var converted = rawData.Select(e => (TmdbMovieInfo) e);
            foreach (TmdbMovieInfo entityInfo in converted)
            {
                double ratingValue = appraisal.CalculateRating(entityInfo);

                var resultInfo = new ResultInfo(entityInfo.ThingId, ratingValue, RatingId);
                ratings.Add(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}.");
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        #endregion
    }
}
