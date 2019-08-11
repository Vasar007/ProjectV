using System;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.MoviesRating
{
    /// <summary>
    /// Expands appraiser behavior and lets process movie data objects.
    /// </summary>
    public abstract class MoviesAppraiser : Appraiser
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(MoviesAppraiser);

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
