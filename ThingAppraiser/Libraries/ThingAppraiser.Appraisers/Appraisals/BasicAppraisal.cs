using System;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="BasicInfo" />.
    /// </summary>
    public sealed class BasicAppraisal : IAppraisal<BasicInfo>
    {
        /// <summary>
        /// Provides min, max and denominator values to normalize vote count value.
        /// </summary>
        private MinMaxDenominator? _voteCountMMD;

        /// <summary>
        /// Provides min, max and denominator values to normalize vote average value.
        /// </summary>
        private MinMaxDenominator? _voteAverageMMD;

        /// <inheritdoc />
        public string RatingName { get; } = "Common rating";


        /// <summary>
        /// Initializes instance with default values.
        /// </summary>
        public BasicAppraisal()
        {
        }

        #region IAppraisal<BasicInfo> Implementation

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
        /// Vote count characteristics do not contains in <paramref name="rawDataContainer" />. -or-
        /// Vote average characteristics do not contains in <paramref name="rawDataContainer" />.
        /// </exception>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            _voteCountMMD = rawDataContainer.GetParameter(nameof(BasicInfo.VoteCount));
            _voteAverageMMD = rawDataContainer.GetParameter(nameof(BasicInfo.VoteAverage));
        }

        /// <summary>
        /// Calculates rating for <see cref="BasicInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>Normalized sum of vote count and vote average values.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="_voteCountMMD" /> is <c>null</c>. -or-
        /// <see cref="_voteAverageMMD" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(BasicInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            if (_voteCountMMD is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(_voteCountMMD)} is not initialized."
                );
            }
            if (_voteAverageMMD is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(_voteAverageMMD)} is not initialized."
                );
            }

            double vcValue = (entity.VoteCount - _voteCountMMD.MinValue) /
                             _voteCountMMD.Denominator;
            double vaValue = (entity.VoteAverage - _voteAverageMMD.MinValue) /
                             _voteAverageMMD.Denominator;

            return vcValue + vaValue;
        }

        #endregion
    }
}
