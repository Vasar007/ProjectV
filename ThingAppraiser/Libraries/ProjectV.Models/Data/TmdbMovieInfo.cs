using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ProjectV.Models.Data
{
    /// <summary>
    /// Concrete data object for TMDb service <see href="https://www.themoviedb.org" />.
    /// </summary>
    public sealed class TmdbMovieInfo : MovieInfo, IEquatable<TmdbMovieInfo>
    {
        /// <inheritdoc />
        public override string Kind { get; } = nameof(TmdbMovieInfo);

        /// <summary>
        /// Popularity rating which calculated by TMDb.
        /// </summary>
        public double Popularity { get; }

        /// <summary>
        /// The status of an adult film (18+).
        /// </summary>
        public bool Adult { get; }

        /// <summary>
        /// Contains all genre ids of movie.
        /// </summary>
        public IReadOnlyList<int> GenreIds { get; }

        /// <summary>
        /// Poster file path to TMDb image service.
        /// </summary>
        public string PosterPath { get; }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="popularity">Internal popularity rating of online-service.</param>
        /// <param name="adult">Defines if film is for adults.</param>
        /// <param name="genreIds">Collection of all genres.</param>
        /// <param name="posterPath">Path to image file on TMDb server.</param>
        [JsonConstructor]
        public TmdbMovieInfo(int thingId, string title, int voteCount, double voteAverage,
            string overview, DateTime releaseDate, double popularity, bool adult,
            IReadOnlyList<int> genreIds, string posterPath)
            : base(thingId, title, voteCount, voteAverage, overview, releaseDate)
        {
            Popularity = popularity;
            Adult = adult;
            GenreIds = genreIds;
            PosterPath = posterPath;
        }

        #region MovieInfo Overridden Methods

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as TmdbMovieInfo);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region IEquatable<TmdbMovieInfo> Implementation

        /// <inheritdoc />
        public bool Equals(TmdbMovieInfo? other)
        {
            if (other is null) return false;

            if (ReferenceEquals(this, other)) return true;

            if (!base.IsEqual(other)) return false;

            return IsEqual(other);
        }

        #endregion

        /// <summary>
        /// Determines whether two specified instances of <see cref="TmdbMovieInfo" /> are equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        public static bool operator ==(TmdbMovieInfo? left, TmdbMovieInfo? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Determines whether two specified instances of <see cref="TmdbMovieInfo" /> are not equal.
        /// </summary>
        /// <param name="left">Left hand side object to compare.</param>
        /// <param name="right">Right hand side object to compare.</param>
        /// <returns>
        /// <c>true</c> if values are not memberwise equals, <c>false</c> otherwise.
        /// </returns>
        public static bool operator !=(TmdbMovieInfo? left, TmdbMovieInfo? right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether specified instance of <see cref="TmdbMovieInfo" /> is equal to caller
        /// object.
        /// </summary>
        /// <param name="other">Other object to compare.</param>
        /// <returns><c>true</c> if values are memberwise equals, <c>false</c> otherwise.</returns>
        private bool IsEqual(TmdbMovieInfo other)
        {
            // Note: list with genre IDs usually has only few items and that is why comparison
            // using contains method is considered the best option here.

            const double tolerance = 1e-6;
            return Math.Abs(Popularity - other.Popularity) < tolerance &&
                   Adult.Equals(other.Adult) &&
                   GenreIds.All(genreId => other.GenreIds.Contains(genreId)) &&
                   string.Equals(PosterPath, other.PosterPath, StringComparison.Ordinal);
        }
    }
}
