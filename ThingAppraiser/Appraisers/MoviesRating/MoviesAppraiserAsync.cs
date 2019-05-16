using System;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class MoviesAppraiserAsync : AppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = "MoviesAppraiserAsync";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(MovieInfo);


        public MoviesAppraiserAsync()
        {
        }
    }
}
