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
        /// <summary>
        /// The appraisal to calculate ratings for <see cref="TmdbMovieInfo" />.
        /// </summary>
        private readonly IAppraisal<TmdbMovieInfo> _appraisal;

        /// <inheritdoc />
        public override string Tag { get; } = nameof(TmdbAppraiser);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName => _appraisal.RatingName;


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        /// <param name="appraisal">The strategy to calculate rating value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraisal" /> is <c>null</c>.
        /// </exception>
        public TmdbAppraiser(IAppraisal<TmdbMovieInfo> appraisal)
        {
            _appraisal = appraisal.ThrowIfNull(nameof(appraisal));
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

            _appraisal.PrepareCalculation(rawDataContainer);

            var converted = rawData.Select(e => (TmdbMovieInfo) e);
            foreach (TmdbMovieInfo entityInfo in converted)
            {
                double ratingValue = _appraisal.CalculateRating(entityInfo);

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
