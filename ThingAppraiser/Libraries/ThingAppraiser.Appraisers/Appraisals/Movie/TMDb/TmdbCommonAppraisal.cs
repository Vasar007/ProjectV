using System;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals.Movie.Tmdb
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="TmdbMovieInfo" />.
    /// </summary>
    public sealed class TmdbCommonAppraisal : IAppraisal<TmdbMovieInfo>
    {
        /// <inheritdoc />
        public string RatingName { get; } = "Rating based on popularity";


        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        public TmdbCommonAppraisal()
        {
        }

        #region IAppraisal<TmdbMovieInfo> Implementation

        /// <summary>
        /// No exctraction will be perfomed because this appraisal no needed in such preparation.
        /// </summary>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Calculates rating for <see cref="TmdbMovieInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>Popularity value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(TmdbMovieInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            double popValue = entity.Popularity;

            return popValue;
        }

        #endregion
    }
}
