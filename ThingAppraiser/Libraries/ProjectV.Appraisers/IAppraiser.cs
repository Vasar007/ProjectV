using System;
using System.Collections.Generic;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers
{
    /// <summary>
    /// Interface to appraiser classes.
    /// </summary>
    public interface IAppraiser : ITagable, ITypeId
    {
        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        string RatingName { get; }

        /// <summary>
        /// Specify rating ID for result.
        /// </summary>
        Guid RatingId { get; set; }


        /// <summary>
        /// Calculates ratings based on specified container values.
        /// ratings.
        /// </summary>
        /// <param name="rawDataContainer">
        /// The entities to appraise with additional parameters.
        /// </param>
        /// <param name="outputResults">The flag to define need to output.</param>
        /// <returns>Collection of result object (data object with rating).</returns>
        IReadOnlyList<ResultInfo> GetRatings(RawDataContainer rawDataContainer,
            bool outputResults);
    }
}
