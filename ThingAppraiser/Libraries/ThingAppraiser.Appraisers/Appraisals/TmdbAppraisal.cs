
using System;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="TmdbMovieInfo" />.
    /// </summary>
    public sealed class TmdbAppraisal : IAppraisal<TmdbMovieInfo>
    {
        /// <summary>
        /// Provides rating calculation for <see cref="BasicInfo" /> part of
        /// <see cref="TmdbMovieInfo" />.
        /// </summary>
        private readonly IAppraisal<BasicInfo> _basicAppraisal;

        /// <summary>
        /// Provides min, max and denominator values to normalize popularity value.
        /// </summary>
        private readonly MinMaxDenominator _popularityMMD;

        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        /// <param name="basicAppraisal">
        /// The basic appraisal to calculate rating for <see cref="BasicInfo" /> part of
        /// <see cref="TmdbMovieInfo" />.
        /// </param>
        /// <param name="popularityMMD">The value to normalize popularity property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="basicAppraisal" /> is <c>null</c> -or-
        /// <paramref name="popularityMMD" /> is <c>null</c> .
        /// </exception>
        public TmdbAppraisal(IAppraisal<BasicInfo> basicAppraisal, MinMaxDenominator popularityMMD)
        {
            _basicAppraisal = basicAppraisal.ThrowIfNull(nameof(basicAppraisal));
            _popularityMMD = popularityMMD.ThrowIfNull(nameof(popularityMMD));
        }

        /// <summary>
        /// Initializes instance with specified valus.
        /// </summary>
        /// <param name="basicAppraisal">
        /// The basic appraisal to calculate rating for <see cref="BasicInfo" /> part of
        /// <see cref="TmdbMovieInfo" />.
        /// </param>
        /// <param name="rawDataContainer">
        /// The object which contains values to normalize properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="basicAppraisal" /> is <c>null</c> -or-
        /// <paramref name="rawDataContainer" /> is <c>null</c> .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Popularity characteristics do not contains in <paramref name="rawDataContainer" />.
        /// </exception>
        public TmdbAppraisal(IAppraisal<BasicInfo> basicAppraisal, RawDataContainer rawDataContainer)
        {
            _basicAppraisal = basicAppraisal.ThrowIfNull(nameof(basicAppraisal));

            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            _popularityMMD = rawDataContainer.GetParameter(nameof(TmdbMovieInfo.Popularity));
        }

        #region IAppraisal<TmdbMovieInfo> Implementation

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
        public double CalculateRating(TmdbMovieInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            double baseValue = _basicAppraisal.CalculateRating(entity);
            double popValue = (entity.Popularity - _popularityMMD.MinValue) /
                              _popularityMMD.Denominator;

            return baseValue + popValue;
        }

        #endregion
    }
}
