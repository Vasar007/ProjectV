using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.MoviesRating.Omdb
{
    /// <summary>
    /// Concrete appraiser for OMDb data.
    /// </summary>
    public sealed class OmdbAppraiser : MoviesAppraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(OmdbAppraiser);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(OmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on Metascore and IMDB rating";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public OmdbAppraiser()
        {
        }

        #region MoviesAppraiser Overriden Methods

        /// <summary>
        /// Calculates rating which depends on Metascore and IMDB rating value.
        /// </summary>
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
            if (!rawData.All(e => e is OmdbMovieInfo))
            {
                throw new ArgumentException(
                    $"One of element type is invalid for appraiser with type {TypeId.FullName}."
                );
            }

            var converted = rawData.Select(e => (OmdbMovieInfo) e);
            foreach (OmdbMovieInfo entityInfo in converted)
            {
                double ratingValue = entityInfo.VoteAverage + (entityInfo.Metascore / 10);

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
