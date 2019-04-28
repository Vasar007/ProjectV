using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Expands basic data object with movie specific fields.
    /// </summary>
    [Serializable]
    public class CMovieInfo : CBasicInfo
    {
        /// <summary>
        /// Brief overview of the movie.
        /// </summary>
        public String Overview { get; }

        /// <summary>
        /// Movie release date.
        /// </summary>
        public DateTime ReleaseDate { get;  }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="overview">Movie description.</param>
        /// <param name="release_date">Movie release date.</param>
        [JsonConstructor]
        public CMovieInfo(Int32 id, String title, Int32 vote_count, Double vote_average,
            String overview, DateTime release_date)
            : base(id, title, vote_count, vote_average)
        {
            Overview = overview;
            ReleaseDate = release_date;
        }
    }
}
