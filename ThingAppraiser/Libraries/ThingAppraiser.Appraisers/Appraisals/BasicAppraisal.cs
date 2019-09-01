using System;
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
        private readonly MinMaxDenominator _voteCountMMD;

        /// <summary>
        /// Provides min, max and denominator values to normalize vote average value.
        /// </summary>
        private readonly MinMaxDenominator _voteAverageMMD;


        /// <summary>
        /// Initializes instance with specified valus.
        /// </summary>
        /// <param name="voteCountMMD">The value to normalize vote count property.</param>
        /// <param name="voteAverageMMD">The value to normalize vote average property.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="voteCountMMD" /> is <c>null</c> -or-
        /// <paramref name="voteAverageMMD" /> is <c>null</c> .
        /// </exception>
        public BasicAppraisal(MinMaxDenominator voteCountMMD, MinMaxDenominator voteAverageMMD)
        {
            _voteCountMMD = voteCountMMD.ThrowIfNull(nameof(voteCountMMD));
            _voteAverageMMD = voteAverageMMD.ThrowIfNull(nameof(voteAverageMMD));
        }

        /// <summary>
        /// Initializes instance with specified valus.
        /// </summary>
        /// <param name="rawDataContainer">
        /// The object which contains values to normalize properties.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rawDataContainer" /> is <c>null</c> .
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Vote count characteristics do not contains in <paramref name="rawDataContainer" /> -or-
        /// Vote average characteristics do not contains in <paramref name="rawDataContainer" />.
        /// </exception>
        public BasicAppraisal(RawDataContainer rawDataContainer)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            _voteCountMMD = rawDataContainer.GetParameter(nameof(BasicInfo.VoteCount));
            _voteAverageMMD = rawDataContainer.GetParameter(nameof(BasicInfo.VoteAverage));
        }

        #region IAppraisal<BasicInfo> Implementation

        /// <summary>
        /// Calculates rating for <see cref="BasicInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>Normalized sum of vote count and vote average values.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(BasicInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            double vcValue = (entity.VoteCount - _voteCountMMD.MinValue) /
                             _voteCountMMD.Denominator;
            double vaValue = (entity.VoteAverage - _voteAverageMMD.MinValue) /
                             _voteAverageMMD.Denominator;

            return vcValue + vaValue;
        }

        #endregion
    }
}
