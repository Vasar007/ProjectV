using System;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.GameRating
{
    /// <summary>
    /// Expands appraiser behavior and lets process game data objects.
    /// </summary>
    public abstract class GameAppraiser : Appraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(GameAppraiser);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(GameInfo);


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public GameAppraiser()
        {
        }
    }
}
