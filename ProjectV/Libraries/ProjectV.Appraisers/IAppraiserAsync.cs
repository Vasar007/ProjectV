using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers
{
    /// <summary>
    /// Interface to appraiser async classes.
    /// </summary>
    public interface IAppraiserAsync : ITagable, ITypeId
    {
        /// <summary>
        /// Rating name which describes rating calculation.
        /// </summary>
        string RatingName { get; }


        /// <summary>
        /// Calculates ratings based on specified container values.
        /// ratings.
        /// </summary>
        /// <param name="entitiesInfoQueue">
        /// The source queue of entities to appraise.
        /// </param>
        ///  <param name="entitiesRatingQueue">
        /// The target queue of entities to send appraised entities.
        /// </param>
        /// <param name="outputResults">The flag to define need to output.</param>
        /// <returns>Status of operation at the end of source queue.</returns>
        RatingDataContainer GetRatings(BasicInfo entityInfo, bool outputResults);
    }
}
