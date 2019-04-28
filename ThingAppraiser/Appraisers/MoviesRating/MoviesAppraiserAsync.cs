using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class CMoviesAppraiserAsync : CAppraiserAsync
    {
        /// <inheritdoc />
        public override String Tag => "MoviesAppraiserAsync";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieInfo);


        public CMoviesAppraiserAsync()
        {
        }
    }
}
