using System;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.GameRating
{
    /// <summary>
    /// Expands appraiser behavior and lets process game data objects asynchronously.
    /// </summary>
    public abstract class GameAppraiserAsync : AppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(GameAppraiserAsync);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(GameInfo);


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public GameAppraiserAsync()
        {
        }
    }
}
