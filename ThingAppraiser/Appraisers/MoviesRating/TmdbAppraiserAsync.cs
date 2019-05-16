using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public class TmdbAppraiserAsync : MoviesAppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = "TmdbAppraiserAsync";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on popularity";


        public TmdbAppraiserAsync()
        {
        }

        #region MoviesAppraiserAsync Overriden Methods

        public override async Task<bool> GetRatings(BufferBlock<BasicInfo> entitiesInfoQueue,
            BufferBlock<RatingDataContainer> entitiesRatingQueue, bool outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                BasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();
                var movieInfo = (TmdbMovieInfo) entityInfo;

                var resultInfo = new RatingDataContainer(entityInfo, movieInfo.Popularity);
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
