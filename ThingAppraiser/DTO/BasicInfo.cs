using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Basic data object to manipulate all kind of information.
    /// </summary>
    /// <remarks>
    /// ATTENTION! Be careful with naming of properties! They must match the values received in 
    /// JSON.
    /// </remarks>
    [Serializable]
    public class CBasicInfo
    {
        /// <summary>
        /// Represents name of the appraised Thing.
        /// </summary>
        public String Title { get; }

        /// <summary>
        /// Unique Thing identifier. We can get this ID from Internet data service.
        /// </summary>
        public Int32 ID { get; }

        /// <summary>
        /// Number of Thing votes.
        /// </summary>
        public Int32 VoteCount { get; }

        /// <summary>
        /// Average vote value.
        /// </summary>
        public Single VoteAverage { get; }


        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="title">Title of the Thing.</param>
        /// <param name="id">Unique ID of the Thing.</param>
        /// <param name="vote_Count">Number of votes.</param>
        /// <param name="vote_Average">Average vote value.</param>
        [JsonConstructor]
        public CBasicInfo(String title, Int32 id, Int32 vote_Count, Single vote_Average)
        {
            Title = title;
            ID = id;
            VoteCount = vote_Count;
            VoteAverage = vote_Average;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override Boolean Equals(Object obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is CBasicInfo basicInfo)) return false;

            const Double eps = 1e-6;
            return Title.IsEqualWithInvariantCulture(basicInfo.Title) &&
                   ID == basicInfo.ID &&
                   VoteCount == basicInfo.VoteCount && 
                   Math.Abs(VoteAverage - basicInfo.VoteAverage) < eps;
        }

        /// <inheritdoc />
        public override Int32 GetHashCode()
        {
            return ID;
        }

        #endregion
    }
}
