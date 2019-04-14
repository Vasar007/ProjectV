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
        /// <param name="release_Date">Movie release date.</param>
        [JsonConstructor]
        public CMovieInfo(String title, Int32 id, Int32 vote_Count, Single vote_Average,
            String overview, DateTime release_Date)
            : base(title, id, vote_Count, vote_Average)
        {
            Overview = overview;
            ReleaseDate = release_Date;
        }
    }
}
