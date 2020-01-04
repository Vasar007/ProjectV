using System;
using Acolyte.Assertions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals.Game.Steam
{
    /// <summary>
    /// Appraisal to get rating for instances of <see cref="SteamGameInfo" /> with normalization.
    /// </summary>
    public sealed class SteamNormalizedAppraisal : IAppraisal<SteamGameInfo>
    {
        /// <summary>
        /// Provides rating calculation for <see cref="BasicInfo" /> part of
        /// <see cref="SteamGameInfo" />.
        /// </summary>
        private readonly IAppraisal<BasicInfo> _basicAppraisal;

        /// <summary>
        /// Provides min, max and denominator values to normalize price value.
        /// </summary>
        private MinMaxDenominator? _priceMMD;

        /// <inheritdoc />
        public string RatingName { get; } = "Rating based on price and votes";


        /// <summary>
        /// Initializes instance with specified values.
        /// </summary>
        /// <param name="basicAppraisal">
        /// The basic appraisal to calculate rating for <see cref="BasicInfo" /> part of
        /// <see cref="SteamGameInfo" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="basicAppraisal" /> is <c>null</c>.
        /// </exception>
        public SteamNormalizedAppraisal(IAppraisal<BasicInfo> basicAppraisal)
        {
            _basicAppraisal = basicAppraisal.ThrowIfNull(nameof(basicAppraisal));

        }

        #region IAppraisal<SteamGameInfo> Implementation

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
            _priceMMD = rawDataContainer.GetParameter(nameof(SteamGameInfo.Price));
        }

        /// <summary>
        /// Calculates rating for <see cref="SteamGameInfo" /> with specified values.
        /// </summary>
        /// <param name="entity">Target value to calculate rating.</param>
        /// <returns>
        /// Sum of rating <see cref="BasicInfo" /> and normalized price value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="_priceMMD" /> is <c>null</c>.
        /// </exception>
        public double CalculateRating(SteamGameInfo entity)
        {
            entity.ThrowIfNull(nameof(entity));

            if (_priceMMD is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(_priceMMD)} is not initialized."
                );
            }

            double baseValue = _basicAppraisal.CalculateRating(entity);
            double priceValue = (decimal.ToDouble(entity.Price) - _priceMMD.MinValue) /
                              _priceMMD.Denominator;

            return baseValue + priceValue;
        }

        #endregion
    }
}
