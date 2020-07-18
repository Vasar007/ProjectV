using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Appraisers.Appraisals;
using ProjectV.Communication;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers
{
    /// <summary>
    /// Concrete appraiser of specified <typeparamref name="T" /> using strategy to calculate
    /// ratings.
    /// </summary>
    /// <typeparam name="T">The data handler type.</typeparam>
    public sealed class Appraiser<T> : IAppraiser, ITagable, ITypeId
        where T : BasicInfo
    {
        /// <summary>
        /// The appraisal to calculate ratings for <typeparamref name="T" />.
        /// </summary>
        private readonly IAppraisal<T> _appraisal;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = $"Appraiser<{typeof(T).Name}>";

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(T);

        #endregion

        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        public string RatingName => _appraisal.RatingName;

        /// <inheritdoc />
        public Guid RatingId { get; set; }


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        /// <param name="appraisal">The strategy to calculate rating value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraisal" /> is <c>null</c>.
        /// </exception>
        public Appraiser(IAppraisal<T> appraisal)
        {
            _appraisal = appraisal.ThrowIfNull(nameof(appraisal));
        }

        /// <summary>
        /// Makes prior analysis through prepare stage for strategy and then uses it to calculate
        /// ratings.
        /// </summary>
        /// <param name="rawDataContainer">
        /// The entities to appraise with additional parameters.
        /// </param>
        /// <param name="outputResults">The flag to define need to output.</param>
        /// <returns>Collection of result object (data object with rating).</returns>
        /// <remarks>
        /// Entities collection must be unique because rating calculation errors can occur in such
        /// situations.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rawDataContainer" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="rawDataContainer" /> contains instances of invalid type for this 
        /// appraiser.
        /// </exception>
        public IReadOnlyList<ResultInfo> GetRatings(RawDataContainer rawDataContainer,
            bool outputResults)
        {
            rawDataContainer.ThrowIfNull(nameof(rawDataContainer));

            CheckRatingId();

            var ratings = new List<ResultInfo>();
            IReadOnlyList<BasicInfo> rawData = rawDataContainer.RawData;
            if (!rawData.Any()) return ratings;

            // Check if list have proper type.
            IReadOnlyList<T> converted = rawData.Select(e =>
            {
                if (!(e is T result))
                {
                    throw new ArgumentException(
                        $"Element \"{e.Title}\" (ID = {e.ThingId.ToString()}) type " +
                        $"\"{e.GetType().FullName}\" is invalid for appraiser with type " +
                        $"\"{TypeId.FullName}\"."
                    );
                }
                return result;
            }).ToList();

            _appraisal.PrepareCalculation(rawDataContainer);

            foreach (T entityInfo in converted)
            {
                double ratingValue = _appraisal.CalculateRating(entityInfo);

                var resultInfo = new ResultInfo(entityInfo.ThingId, ratingValue, RatingId);
                ratings.Add(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}.");
                }
            }

            ratings.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue));
            return ratings;
        }

        /// <summary>
        /// Checks that class was registered rating and has proper rating ID.
        /// </summary>
        private void CheckRatingId()
        {
            if (RatingId == Guid.Empty)
            {
                throw new InvalidOperationException("Rating ID has no value.");
            }
        }
    }
}
