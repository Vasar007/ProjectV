using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Models.Data
{
    /// <summary>
    /// Basic data object to manipulate all kind of information.
    /// </summary>
    /// <remarks>
    /// ATTENTION! Be careful with naming of parameters! They must match the values received in 
    /// JSON.
    /// </remarks>
    public class BasicInfo : IDataType
    {
        /// <summary>
        /// Unique Thing identifier. We can get this ID from Internet data service.
        /// </summary>
        public int ThingId { get; }

        /// <summary>
        /// Represents name of the appraised Thing.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Number of Thing votes.
        /// </summary>
        public int VoteCount { get; }

        /// <summary>
        /// Average vote value.
        /// </summary>
        public double VoteAverage { get; }

        #region IDataType Implementation

        /// <summary>
        /// Represents kind of additional value. This property used only for JSON (de)serialization.
        /// </summary>
        public virtual string Kind { get; } = nameof(BasicInfo);

        #endregion

        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="thingId">Unique ID of the Thing.</param>
        /// <param name="title">Title of the Thing.</param>
        /// <param name="voteCount">Number of votes.</param>
        /// <param name="voteAverage">Average vote value.</param>
        [JsonConstructor]
        public BasicInfo(int thingId, string title, int voteCount, double voteAverage)
        {
            ThingId = thingId;
            Title = title;
            VoteCount = voteCount;
            VoteAverage = voteAverage;
        }

        #region Object Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is BasicInfo basicInfo)) return false;

            const double eps = 1e-6;
            return ThingId == basicInfo.ThingId &&
                   Title.IsEqualWithInvariantCulture(basicInfo.Title) &&
                   VoteCount == basicInfo.VoteCount && 
                   Math.Abs(VoteAverage - basicInfo.VoteAverage) < eps;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ThingId;
        }

        #endregion
    }
}
