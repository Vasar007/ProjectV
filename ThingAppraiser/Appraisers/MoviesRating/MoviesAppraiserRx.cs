using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class CMoviesAppraiserRx : CAppraiserRx
    {
        /// <inheritdoc />
        public override String Tag => "MoviesAppraiserRx";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieInfo);


        public CMoviesAppraiserRx()
        {
        }
    }
}
