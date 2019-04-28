using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Basic data object to manipulate all kind of information.
    /// </summary>
    /// <remarks>
    /// ATTENTION! Be careful with naming of parameters! They must match the values received in 
    /// JSON.
    /// </remarks>
    [Serializable]
    public class CBasicInfo
    {
        /// <summary>
        /// Unique Thing identifier. We can get this ID from Internet data service.
        /// </summary>
        public Int32 ThingID { get; }

        /// <summary>
        /// Represents name of the appraised Thing.
        /// </summary>
        public String Title { get; }

        /// <summary>
        /// Number of Thing votes.
        /// </summary>
        public Int32 VoteCount { get; }

        /// <summary>
        /// Average vote value.
        /// </summary>
        public Double VoteAverage { get; }


        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="id">Unique ID of the Thing.</param>
        /// <param name="title">Title of the Thing.</param>
        /// <param name="vote_count">Number of votes.</param>
        /// <param name="vote_average">Average vote value.</param>
        [JsonConstructor]
        public CBasicInfo(Int32 id, String title, Int32 vote_count, Double vote_average)
        {
            ThingID = id;
            Title = title;
            VoteCount = vote_count;
            VoteAverage = vote_average;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override Boolean Equals(Object obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is CBasicInfo basicInfo)) return false;

            const Double eps = 1e-6;
            return ThingID == basicInfo.ThingID &&
                   Title.IsEqualWithInvariantCulture(basicInfo.Title) &&
                   VoteCount == basicInfo.VoteCount && 
                   Math.Abs(VoteAverage - basicInfo.VoteAverage) < eps;
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            return ThingID;
        }

        #endregion
    }
}
