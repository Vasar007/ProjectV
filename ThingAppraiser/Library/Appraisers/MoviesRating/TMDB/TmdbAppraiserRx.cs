using System;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public class TmdbAppraiserRx : MoviesAppraiserRx
    {
        /// <inheritdoc />
        public override string Tag { get; } = "TmdbAppraiserRx";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on popularity";


        public TmdbAppraiserRx()
        {
        }

        #region MoviesAppraiserRx Overriden Methods

        public override RatingDataContainer GetRatings(BasicInfo entityInfo, bool outputResults)
        {
            var movieInfo = (TmdbMovieInfo) entityInfo;

            var resultInfo = new RatingDataContainer(entityInfo, movieInfo.Popularity);

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}");
            }

            return resultInfo;
        }

        #endregion
    }
}
