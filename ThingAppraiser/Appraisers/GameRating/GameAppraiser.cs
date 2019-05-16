using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Expands appraiser behavior and lets process game data objects.
    /// </summary>
    public abstract class GameAppraiser : Appraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = "GameAppraiser";

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
