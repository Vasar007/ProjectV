using System;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Expands basic data object with game specific fields.
    /// </summary>
    public class GameInfo : BasicInfo
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
    }
}
