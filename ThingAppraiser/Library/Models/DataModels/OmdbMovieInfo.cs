using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Concrete data object for OMDb service <see href="http://www.omdbapi.com/" />.
    /// </summary>
    public class OmdbMovieInfo : MovieInfo
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
        public List<string> GenreIds { get; }

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
    }
}
