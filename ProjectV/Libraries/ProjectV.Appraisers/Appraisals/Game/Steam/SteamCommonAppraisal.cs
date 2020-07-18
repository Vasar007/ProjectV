using System;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using Acolyte.Assertions;

namespace ProjectV.Appraisers.Appraisals.Game.Steam
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="SteamGameInfo" />.
    /// </summary>
    public sealed class SteamCommonAppraisal : IAppraisal<SteamGameInfo>
    {
        /// <inheritdoc />
        public string RatingName { get; } =
            "Rating based on subtraction of final and initial price";


        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        public SteamCommonAppraisal()
        {
        }

        #region IAppraisal<SteamGameInfo> Implementation

        /// <summary>
        /// No exctraction will be perfomed because this appraisal no needed in such preparation.
        /// </summary>
        public void PrepareCalculation(RawDataContainer rawDataContainer)
        {
            // Nothing to do.
        }

        /// <summary>
        /// Calculates rating for <see cref="SteamGameInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>
        /// Subtraction of final and initial price.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(SteamGameInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            double priceValue = decimal.ToDouble(entity.Price) - entity.VoteAverage;

            return priceValue;
        }

        #endregion
    }
}
