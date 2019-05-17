using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.FuzzyLogicSystem;

namespace ThingAppraiser.Appraisers
{
    public class FuzzyTmdbAppraiserAsync : MoviesAppraiserAsync
    {
        private readonly IFuzzyController _fuzzyController = new FuzzyControllerIFuzzyController();

        /// <inheritdoc />
        public override string Tag { get; } = "FuzzyTmdbAppraiserAsync";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


        public FuzzyTmdbAppraiserAsync()
        {
        }

        #region MoviesAppraiserAsync Overriden Methods

        public override async Task<bool> GetRatings(BufferBlock<BasicInfo> entitiesInfoQueue,
            BufferBlock<RatingDataContainer> entitiesRatingQueue, bool outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                BasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();

                var movieTmdbInfo = (TmdbMovieInfo) entityInfo;
                double rating = _fuzzyController.CalculateRating(
                    movieTmdbInfo.VoteCount, movieTmdbInfo.VoteAverage,
                    movieTmdbInfo.ReleaseDate.Year, movieTmdbInfo.Popularity,
                    movieTmdbInfo.Adult ? 1 : 0
                );

                var resultInfo = new RatingDataContainer(entityInfo, rating);

                await entitiesRatingQueue.SendAsync(resultInfo);
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }
            return true;
        }

        #endregion
    }
}
