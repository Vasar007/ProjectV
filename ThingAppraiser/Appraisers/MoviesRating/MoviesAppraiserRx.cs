using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class MoviesAppraiserRx : AppraiserRx
    {
        /// <inheritdoc />
        public override string Tag { get; } = "MoviesAppraiserRx";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(MovieInfo);


        public MoviesAppraiserRx()
        {
        }
    }
}
