using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Expands basic data object with movie specific fields.
    /// </summary>
    public class MovieInfo : BasicInfo
    {
        /// <inheritdoc />
        public override string Kind { get; } = nameof(MovieInfo);

        /// <summary>
        /// Brief overview of the movie.
        /// </summary>
        public string Overview { get; }

        /// <summary>
        /// Movie release date.
        /// </summary>
        public DateTime ReleaseDate { get;  }


        /// <inheritdoc />
        /// <summary>
        /// Initializes instance with given parameters.
        /// </summary>
        /// <param name="overview">Movie description.</param>
        /// <param name="releaseDate">Movie release date.</param>
        [JsonConstructor]
        public MovieInfo(int thingId, string title, int voteCount, double voteAverage,
            string overview, DateTime releaseDate)
            : base(thingId, title, voteCount, voteAverage)
        {
            Overview = overview;
            ReleaseDate = releaseDate;
        }
    }
}
