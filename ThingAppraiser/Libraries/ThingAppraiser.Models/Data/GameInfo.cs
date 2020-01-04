using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Models.Data
{
    /// <summary>
    /// Expands basic data object with game specific fields.
    /// </summary>
    public class GameInfo : BasicInfo, IEquatable<GameInfo>
    {
        /// <inheritdoc />
        public override string Kind { get; } = nameof(GameInfo);

        /// <summary>
        /// Brief overview of the game.
        /// </summary>
        public string Overview { get; }

        /// <summary>
        /// Game release date.
        /// </summary>
        public DateTime ReleaseDate { get; }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="overview">Game description.</param>
        /// <param name="releaseDate">Game release date.</param>
        [JsonConstructor]
        public GameInfo(int thingId, string title, int voteCount, double voteAverage,
            string overview, DateTime releaseDate)
            : base(thingId, title, voteCount, voteAverage)
        {
            Overview = overview;
            ReleaseDate = releaseDate;
        }

        #region BasicInfo Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is GameInfo other)) return false;

            return IsEqual(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region IEquatable<GameInfo> Implementation

        /// <inheritdoc />
        public bool Equals(GameInfo? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            if (!base.IsEqual(other)) return false;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="GameInfo" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(GameInfo? left, GameInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="GameInfo" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(GameInfo? left, GameInfo? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="GameInfo" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        protected bool IsEqual(GameInfo other)
        {
            return ReleaseDate.Equals(other.ReleaseDate) &&
                   string.Equals(Overview, other.Overview, StringComparison.Ordinal);
        }
    }
}
