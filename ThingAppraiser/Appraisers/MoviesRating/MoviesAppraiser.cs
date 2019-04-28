using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Expands appraiser behavior and lets process movie data objects.
    /// </summary>
    public abstract class CMoviesAppraiser : CAppraiser
    {
        /// <inheritdoc />
        public override String Tag => "MoviesAppraiser";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieInfo);


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CMoviesAppraiser()
        {
        }
    }
}
