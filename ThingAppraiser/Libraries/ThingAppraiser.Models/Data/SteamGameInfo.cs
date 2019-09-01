using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ThingAppraiser.Models.Data
{
    /// <summary>
    /// Concrete data object for Steam service <see href="https://store.steampowered.com/" />.
    /// </summary>
    public sealed class SteamGameInfo : GameInfo, IEquatable<SteamGameInfo>
    {
        /// <inheritdoc />
        public override string Kind { get; } = nameof(SteamGameInfo);

        /// <summary>
        /// Price of the game.
        /// </summary>
        public decimal Price { get; }

        /// <summary>
        /// Required age for game.
        /// </summary>
        public int RequiredAge { get; }

        /// <summary>
        /// Contains all genre ids of game.
        /// </summary>
        public IReadOnlyList<int> GenreIds { get; }

        /// <summary>
        /// Poster file path to Steam image service.
        /// </summary>
        public string PosterPath { get; }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="price">Price of the game.</param>
        /// <param name="requiredAge">Defines required age for game.</param>
        /// <param name="genreIds">Collection of all genres.</param>
        /// <param name="posterPath">Path to image file on Steam server.</param>
        [JsonConstructor]
        public SteamGameInfo(int thingId, string title, int voteCount, double voteAverage,
            string overview, DateTime releaseDate, decimal price, int requiredAge,
            List<int> genreIds, string posterPath)
            : base(thingId, title, voteCount, voteAverage, overview, releaseDate)
        {
            Price = price;
            RequiredAge = requiredAge;
            GenreIds = genreIds;
            PosterPath = posterPath;
        }

        #region GameInfo Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is SteamGameInfo other)) return false;

            return IsEqual(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region IEquatable<SteamGameInfo> Implementation

        /// <inheritdoc />
        public bool Equals(SteamGameInfo? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            if (!base.IsEqual(other)) return false;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="SteamGameInfo" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(SteamGameInfo? left, SteamGameInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="SteamGameInfo" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(SteamGameInfo? left, SteamGameInfo? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="SteamGameInfo" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        private bool IsEqual(SteamGameInfo other)
        {
            // Note: list with genre IDs usually has only few items and that is why comparison
            // using contains method is considered the best option here.

            return Price.Equals(other.Price) &&
                   RequiredAge.Equals(other.RequiredAge) &&
                   GenreIds.All(genreId => other.GenreIds.Contains(genreId)) &&
                   string.Equals(PosterPath, other.PosterPath, StringComparison.InvariantCulture);
        }
    }
}
