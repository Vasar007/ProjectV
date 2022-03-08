using System;
using Acolyte.Assertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers.Appraisals
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="BasicInfo" />.
    /// </summary>
    public sealed class BasicAppraisalCommon : IAppraisal<BasicInfo>
    {
        /// <inheritdoc />
        public string RatingName { get; } = "Common rating";


        /// <summary>
        /// Initializes instance with default values.
        /// </summary>
        public BasicAppraisalCommon()
        {
        }

        #region IAppraisal<BasicInfo> Implementation

        /// <summary>
        /// No exctraction will be perfomed because this appraisal no needed in such preparation.
        /// </summary>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Calculates rating for <see cref="BasicInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>Average vote value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(BasicInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            return entity.VoteAverage;
        }

        #endregion
    }
}
