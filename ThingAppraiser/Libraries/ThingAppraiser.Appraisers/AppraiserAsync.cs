using System;
using ThingAppraiser.Appraisers.Appraisals;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Concrete async appraiser of specified <typeparamref name="T" /> using strategy to calculate
    /// ratings.
    /// </summary>
    /// <typeparam name="T">The data handler type.</typeparam>
    public sealed class AppraiserAsync<T> : IAppraiserAsync, ITagable, ITypeId
        where T : BasicInfo
    {
        /// <summary>
        /// The appraisal to calculate ratings for <typeparamref name="T" />.
        /// </summary>
        private readonly IAppraisal<T> _appraisal;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = $"AppraiserAsync<{typeof(T).Name}>";
        
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
        public AppraiserAsync(IAppraisal<T> appraisal)
        {
            _appraisal = appraisal.ThrowIfNull(nameof(appraisal));
        }

        public RatingDataContainer GetRatings(BasicInfo entityInfo, bool outputResults)
        {
            if (!(entityInfo is T convertedInfo))
            {
                throw new ArgumentException(
                    $"Element \"{entityInfo.Title}\" (ID = {entityInfo.ThingId.ToString()}) " +
                    $"type \"{entityInfo.GetType().FullName}\" is invalid for appraiser with " +
                    $"type \"{TypeId.FullName}\"."
                );
            }

            double ratingValue = _appraisal.CalculateRating(convertedInfo);

            var resultInfo = new RatingDataContainer(entityInfo, ratingValue, RatingId);

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage(resultInfo.ToString());
            }

            return resultInfo;
        }
    }
}
