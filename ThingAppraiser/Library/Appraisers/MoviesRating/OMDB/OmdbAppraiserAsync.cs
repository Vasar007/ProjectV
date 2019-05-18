using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public class OmdbAppraiserAsync : MoviesAppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = "OmdbAppraiserAsync";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(OmdbMovieInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on IMDB rating value";


        public OmdbAppraiserAsync()
        {
        }

        #region MoviesAppraiserAsync Overriden Methods

        public override async Task<bool> GetRatings(BufferBlock<BasicInfo> entitiesInfoQueue,
            BufferBlock<RatingDataContainer> entitiesRatingQueue, bool outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                BasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();
                var gameInfo = (OmdbMovieInfo) entityInfo;

                var resultInfo = new RatingDataContainer(entityInfo, gameInfo.VoteAverage);
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
