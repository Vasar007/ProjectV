
using System;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals.Movie.Tmdb
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="TmdbMovieInfo" /> with normalization.
    /// </summary>
    public sealed class TmdbNormalizedAppraisal : IAppraisal<TmdbMovieInfo>
    {
        /// <summary>
        /// Provides rating calculation for <see cref="BasicInfo" /> part of
        /// <see cref="TmdbMovieInfo" />.
        /// </summary>
        private readonly IAppraisal<BasicInfo> _basicAppraisal;

        /// <summary>
        /// Provides min, max and denominator values to normalize popularity value.
        /// </summary>
        private MinMaxDenominator? _popularityMMD;

        /// <inheritdoc />
        public string RatingName { get; } = "Rating based on popularity and votes";


        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        /// <param name="basicAppraisal">
        /// The basic appraisal to calculate rating for <see cref="BasicInfo" /> part of
        /// <see cref="TmdbMovieInfo" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="basicAppraisal" /> is <c>null</c>.
        /// </exception>
        public TmdbNormalizedAppraisal(IAppraisal<BasicInfo> basicAppraisal)
        {
            _basicAppraisal = basicAppraisal.ThrowIfNull(nameof(basicAppraisal));

        }

        #region IAppraisal<TmdbMovieInfo> Implementation

        /// <summary>
        /// Extracts additional values to rating calculation from <see cref="RawDataContainer" />.
        /// </summary>
        /// <param name="rawDataContainer">
        /// The object which contains values to normalize properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rawDataContainer" /> is <c>null</c> .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Popularity characteristics do not contains in <paramref name="rawDataContainer" />.
        /// </exception>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            _basicAppraisal.PrepareCalculation(rawDataContainer);
            _popularityMMD = rawDataContainer.GetParameter(nameof(TmdbMovieInfo.Popularity));
        }

        /// <summary>
        /// Calculates rating for <see cref="TmdbMovieInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>
        /// Sum of rating <see cref="BasicInfo" /> and normalized popularity value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="_popularityMMD" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(TmdbMovieInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            if (_popularityMMD is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(_popularityMMD)} is not initialized."
                );
            }

            double baseValue = _basicAppraisal.CalculateRating(entity);
            double popValue = (entity.Popularity - _popularityMMD.MinValue) /
                              _popularityMMD.Denominator;

            return baseValue + popValue;
        }

        #endregion
    }
}
