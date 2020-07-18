using System;
using Acolyte.Assertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers.Appraisals.Movie.Omdb
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="OmdbMovieInfo" />.
    /// </summary>
    public sealed class OmdbCommonAppraisal : IAppraisal<OmdbMovieInfo>
    {
        /// <inheritdoc />
        public string RatingName { get; } = "Rating based on IMDB rating value";


        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        public OmdbCommonAppraisal()
        {

        }

        #region IAppraisal<OmdbMovieInfo> Implementation

        /// <summary>
        /// No exctraction will be perfomed because this appraisal no needed in such preparation.
        /// </summary>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Calculates rating for <see cref="OmdbMovieInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>
        /// Sum of rating <see cref="BasicInfo" /> and normalized metascore value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(OmdbMovieInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            double imdbValue = entity.VoteAverage;

            return imdbValue;
        }

        #endregion
    }
}
