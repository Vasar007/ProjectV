using System;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.MoviesRating
{
    public abstract class MoviesAppraiserAsync : AppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(MoviesAppraiserAsync);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(MovieInfo);


        public MoviesAppraiserAsync()
        {
        }
    }
}
