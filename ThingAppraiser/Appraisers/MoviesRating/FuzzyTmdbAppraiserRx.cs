using System;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.FuzzyLogicSystem;

namespace ThingAppraiser.Appraisers
{
    public class FuzzyTmdbAppraiserRx : MoviesAppraiserRx
    {
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public override string Tag { get; } = "FuzzyTmdbAppraiserRx";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


        public FuzzyTmdbAppraiserRx()
        {
        }

        #region MoviesAppraiserRx Overriden Methods

        public override RatingDataContainer GetRatings(BasicInfo entityInfo, bool outputResults)
        {
            var movieTmdbInfo = (TmdbMovieInfo) entityInfo;
            double rating = _fuzzyController.CalculateRating(
                movieTmdbInfo.VoteCount, movieTmdbInfo.VoteAverage,
                movieTmdbInfo.ReleaseDate.Year, movieTmdbInfo.Popularity,
                movieTmdbInfo.Adult ? 1 : 0
            );

            var resultInfo = new RatingDataContainer(entityInfo, rating);

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage(resultInfo.ToString());
            }

            return resultInfo;
        }

        #endregion
    }
}
