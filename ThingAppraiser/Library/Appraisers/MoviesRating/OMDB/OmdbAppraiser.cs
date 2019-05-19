using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Concrete appraiser for OMDB data.
    /// </summary>
    public class OmdbAppraiser : MoviesAppraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = "OmdbAppraiser";

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
        public override ResultList GetRatings(RawDataContainer rawDataContainer, bool outputResults)
        {
            CheckRatingId();

            var ratings = new ResultList();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.GetData();
            if (rawData.IsNullOrEmpty()) return ratings;

            // Check if list have proper type.
            if (!rawData.All(e => e is OmdbMovieInfo))
            {
                throw new ArgumentException(
                    $"Element type is invalid for appraiser with type {TypeId.FullName}"
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
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}");
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        #endregion
    }
}
