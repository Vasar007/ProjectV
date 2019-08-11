using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.MoviesRating.Omdb
{
    public class OmdbAppraiserAsync : MoviesAppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(OmdbAppraiserAsync);

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
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}");
                }
            }
            return true;
        }

        #endregion
    }
}
