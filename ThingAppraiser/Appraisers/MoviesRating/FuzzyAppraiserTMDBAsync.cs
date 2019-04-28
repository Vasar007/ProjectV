using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.FuzzyLogicSystem;

namespace ThingAppraiser.Appraisers
{
    public class CFuzzyAppraiserTMDBAsync : CMoviesAppraiserAsync
    {
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public override String Tag => "FuzzyAppraiserTMDBAsync";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieTMDBInfo);


        public CFuzzyAppraiserTMDBAsync()
        {
        }

        #region CMoviesAppraiserAsync Overriden Methods

        public override async Task<Boolean> GetRatings(BufferBlock<CBasicInfo> entitiesInfoQueue,
            BufferBlock<CRatingDataContainer> entitiesRatingQueue, Boolean outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                CBasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();

                var movieTMDBInfo = (CMovieTMDBInfo) entityInfo;
                Double rating = _fuzzyController.CalculateRating(
                    movieTMDBInfo.VoteCount, movieTMDBInfo.VoteAverage,
                    movieTMDBInfo.ReleaseDate.Year, movieTMDBInfo.Popularity,
                    movieTMDBInfo.Adult ? 1 : 0
                );

                var resultInfo = new CRatingDataContainer(entityInfo, rating);

                await entitiesRatingQueue.SendAsync(resultInfo);
                if (outputResults)
                {
                    SGlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }
            return true;
        }

        #endregion
    }
}
