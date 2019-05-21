using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ThingAppraiser.Data
{
    /// <summary>
    /// Concrete data object for Steam service <see href="https://store.steampowered.com/" />.
    /// </summary>
    public class SteamGameInfo : GameInfo
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
        public List<int> GenreIds { get; }

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
    }
}
