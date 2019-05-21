using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Concrete data object for TMDb service <see href="https://www.themoviedb.org" />.
    /// </summary>
    public class TmdbMovieInfo : MovieInfo
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
        public List<int> GenreIds { get; }

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
            List<int> genreIds, string posterPath)
            : base(thingId, title, voteCount, voteAverage, overview, releaseDate)
        {
            Popularity = popularity;
            Adult = adult;
            GenreIds = genreIds;
            PosterPath = posterPath;
        }
    }
}
