using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Expands appraiser behavior and lets process movie data objects.
    /// </summary>
    public abstract class MoviesAppraiser : Appraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = "MoviesAppraiser";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(MovieInfo);


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public MoviesAppraiser()
        {
        }
    }
}
