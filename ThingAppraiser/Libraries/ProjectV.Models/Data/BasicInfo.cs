using System;
using Newtonsoft.Json;

namespace ProjectV.Models.Data
{
    /// <summary>
    /// Basic data object to manipulate all kind of information.
    /// </summary>
    /// <remarks>
    /// ATTENTION! Be careful with naming of parameters! They must match the values received in 
    /// JSON.
    /// </remarks>
    public class BasicInfo : IDataType, IEquatable<BasicInfo>
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
        public override bool Equals(object? obj)
        {
            return Equals(obj as BasicInfo);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(ThingId);
        }

        #endregion

        #region IEquatable<BasicInfo> Implementation

        /// <inheritdoc />
        public bool Equals(BasicInfo? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="BasicInfo" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(BasicInfo? left, BasicInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="BasicInfo" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(BasicInfo? left, BasicInfo? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="BasicInfo" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        protected bool IsEqual(BasicInfo other)
        {
            const double tolerance = 1e-6;
            return ThingId.Equals(other.ThingId) &&
                   string.Equals(Title, other.Title, StringComparison.Ordinal) &&
                   VoteCount.Equals(other.VoteCount) &&
                   Math.Abs(VoteAverage - other.VoteAverage) < tolerance;
        }
    }
}
