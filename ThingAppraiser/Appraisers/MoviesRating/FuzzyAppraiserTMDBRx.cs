using System;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.FuzzyLogicSystem;

namespace ThingAppraiser.Appraisers
{
    public class CFuzzyAppraiserTMDBRx : CMoviesAppraiserRx
    {
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public override String Tag => "FuzzyAppraiserTMDBRx";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieTMDBInfo);


        public CFuzzyAppraiserTMDBRx()
        {
        }

        #region CMoviesAppraiserRx Overriden Methods

        public override CRatingDataContainer GetRatings(CBasicInfo entityInfo, Boolean outputResults)
        {
            var movieTMDBInfo = (CMovieTMDBInfo) entityInfo;
            Double rating = _fuzzyController.CalculateRating(
                movieTMDBInfo.VoteCount, movieTMDBInfo.VoteAverage,
                movieTMDBInfo.ReleaseDate.Year, movieTMDBInfo.Popularity,
                movieTMDBInfo.Adult ? 1 : 0
            );

            var resultInfo = new CRatingDataContainer(entityInfo, rating);

            if (outputResults)
            {
                SGlobalMessageHandler.OutputMessage(resultInfo.ToString());
            }

            return resultInfo;
        }

        #endregion
    }
}
