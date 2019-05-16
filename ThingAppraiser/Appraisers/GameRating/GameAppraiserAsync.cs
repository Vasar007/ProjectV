using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Expands appraiser behavior and lets process game data objects asynchronously.
    /// </summary>
    public abstract class GameAppraiserAsync : AppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = "GameAppraiserAsync";

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
