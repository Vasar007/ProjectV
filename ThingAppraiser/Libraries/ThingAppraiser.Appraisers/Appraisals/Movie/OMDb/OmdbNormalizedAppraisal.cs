using System;
using Acolyte.Assertions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals.Movie.Omdb
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="OmdbMovieInfo" /> with normalization.
    /// </summary>
    public sealed class OmdbNormalizedAppraisal : IAppraisal<OmdbMovieInfo>
    {
        /// <summary>
        /// Provides rating calculation for <see cref="BasicInfo" /> part of
        /// <see cref="OmdbMovieInfo" />.
        /// </summary>
        private readonly IAppraisal<BasicInfo> _basicAppraisal;

        /// <summary>
        /// Provides min, max and denominator values to normalize metascore value.
        /// </summary>
        private MinMaxDenominator? _metascoreMMD;

        /// <inheritdoc />
        public string RatingName { get; } = "Rating based on Metascore and IMDB rating";


        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        /// <param name="basicAppraisal">
        /// The basic appraisal to calculate rating for <see cref="BasicInfo" /> part of
        /// <see cref="OmdbMovieInfo" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="basicAppraisal" /> is <c>null</c>.
        /// </exception>
        public OmdbNormalizedAppraisal(IAppraisal<BasicInfo> basicAppraisal)
        {
            _basicAppraisal = basicAppraisal.ThrowIfNull(nameof(basicAppraisal));

        }

        #region IAppraisal<OmdbMovieInfo> Implementation

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
            _metascoreMMD = rawDataContainer.GetParameter(nameof(OmdbMovieInfo.Metascore));
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
        /// <exception cref="InvalidOperationException">
        /// <see cref="_metascoreMMD" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(OmdbMovieInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            if (_metascoreMMD is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(_metascoreMMD)} is not initialized."
                );
            }

            double baseValue = _basicAppraisal.CalculateRating(entity);
            double metacriticaValue = (entity.Metascore - _metascoreMMD.MinValue) /
                                      _metascoreMMD.Denominator;

            return baseValue + metacriticaValue;
        }

        #endregion
    }
}
