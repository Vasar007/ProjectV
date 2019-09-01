using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ThingAppraiser.Models.Data
{
    /// <summary>
    /// Concrete data object for OMDb service <see href="http://www.omdbapi.com/" />.
    /// </summary>
    public sealed class OmdbMovieInfo : MovieInfo, IEquatable<OmdbMovieInfo>
    {
        /// <inheritdoc />
        public override string Kind { get; } = nameof(OmdbMovieInfo);

        /// <summary>
        /// Popularity rating which calculated by Metacritica.
        /// </summary>
        public int Metascore { get; }

        /// <summary>
        /// The status of an adult film (18+).
        /// </summary>
        public string Rated { get; }

        /// <summary>
        /// Contains all genre ids of movie.
        /// </summary>
        public IReadOnlyList<string> GenreIds { get; }

        /// <summary>
        /// Poster file path to OMDb image service.
        /// </summary>
        public string PosterPath { get; }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="metascore">Internal popularity rating of online-service.</param>
        /// <param name="rated">Defines rate for the movie</param>
        /// <param name="genreIds">Collection of all genres.</param>
        /// <param name="posterPath">Path to image file on OMDb server.</param>
        [JsonConstructor]
        public OmdbMovieInfo(int thingId, string title, int voteCount, double voteAverage,
            string overview, DateTime releaseDate, int metascore, string rated,
            List<string> genreIds, string posterPath)
            : base(thingId, title, voteCount, voteAverage, overview, releaseDate)
        {
            Metascore = metascore;
            Rated = rated;
            GenreIds = genreIds;
            PosterPath = posterPath;
        }

        #region MovieInfo Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (!(obj is OmdbMovieInfo other)) return false;

            return IsEqual(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region IEquatable<OmdbMovieInfo> Implementation

        /// <inheritdoc />
        public bool Equals(OmdbMovieInfo? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            if (!base.IsEqual(other)) return false;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="OmdbMovieInfo" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(OmdbMovieInfo? left, OmdbMovieInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="OmdbMovieInfo" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(OmdbMovieInfo? left, OmdbMovieInfo? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="OmdbMovieInfo" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        private bool IsEqual(OmdbMovieInfo other)
        {
            // Note: list with genre IDs usually has only few items and that is why comparison
            // using contains method is considered the best option here.

            return string.Equals(Rated, other.Rated, StringComparison.InvariantCulture) &&
                   Metascore.Equals(other.Metascore) &&
                   GenreIds.All(genreId => other.GenreIds.Contains(genreId)) &&
                   string.Equals(PosterPath, other.PosterPath, StringComparison.InvariantCulture);
        }
    }
}
