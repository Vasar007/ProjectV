using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.Appraisals
{
    /// <summary>
    /// Provides method to calculate ratings for instances of <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">The type of items to calcualte rating.</typeparam>
    public interface IAppraisal<T>
        where T : BasicInfo
    {
        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        string RatingName { get; }


        /// <summary>
        /// Extracts additional values to rating calculation from <see cref="RawDataContainer" />.
        /// </summary>
        /// <param name="rawDataContainer">The container to get helper values from.</param>
        void PrepareCalculation(RawDataContainer rawDataContainer);

        /// <summary>
        /// Calculates rating for <typeparamref name="T" />.
        /// </summary>
        /// <param name="entity">The target value to calculate rating.</param>
        /// <returns>Calculated rating value for specified parameter.</returns>
        double CalculateRating(T entity);
    }
}
