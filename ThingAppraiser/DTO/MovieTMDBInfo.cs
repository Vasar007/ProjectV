using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <inheritdoc />
    /// <summary>
    /// Concrete data object for TMDB service <see href="https://www.themoviedb.org" />.
    /// </summary>
    [Serializable]
    public class CMovieTMDBInfo : CMovieInfo
    {
        /// <summary>
        /// Popularity rating which calculated by TMDB.
        /// </summary>
        public Double Popularity { get; }

        /// <summary>
        /// The status of an adult film (18+).
        /// </summary>
        public Boolean Adult { get; }

        /// <summary>
        /// Contains all genre ids of movie.
        /// </summary>
        public List<Int32> GenreIDs { get; }

        /// <summary>
        /// Poster file path to TMDB image service.
        /// </summary>
        public String PosterPath { get; }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="popularity">Internal popularity rating of online-service.</param>
        /// <param name="adult">Defines if film is for adults.</param>
        /// <param name="genre_ids">Collection of all genres.</param>
        /// <param name="poster_path">Path to image file on TMDB server.</param>
        [JsonConstructor]
        public CMovieTMDBInfo(Int32 id, String title, Int32 vote_count, Double vote_average,
            String overview, DateTime release_date, Double popularity, Boolean adult,
            List<Int32> genre_ids, String poster_path)
            : base(id, title, vote_count, vote_average, overview, release_date)
        {
            Popularity = popularity;
            Adult = adult;
            GenreIDs = genre_ids;
            PosterPath = poster_path;
        }
    }
}
