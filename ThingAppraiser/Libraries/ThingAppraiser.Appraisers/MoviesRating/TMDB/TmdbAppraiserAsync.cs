using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.MoviesRating.Tmdb
{
    public sealed class TmdbAppraiserAsync : MoviesAppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(TmdbAppraiserAsync);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on popularity";


        public TmdbAppraiserAsync()
        {
        }

        #region MoviesAppraiserAsync Overriden Methods

        public override async Task<bool> GetRatings(ISourceBlock<BasicInfo> entitiesInfoQueue,
            ITargetBlock<RatingDataContainer> entitiesRatingQueue, bool outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                BasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();
                var movieInfo = (TmdbMovieInfo) entityInfo;

                var resultInfo = new RatingDataContainer(entityInfo, movieInfo.Popularity);
                await entitiesRatingQueue.SendAsync(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}");
                }
            }
            return true;
        }

        #endregion
    }
}
