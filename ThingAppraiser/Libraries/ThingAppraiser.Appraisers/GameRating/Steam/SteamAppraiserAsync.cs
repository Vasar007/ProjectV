using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers.GameRating.Steam
{
    public sealed class SteamAppraiserAsync : GameAppraiserAsync
    {
        /// <inheritdoc />
        public override string Tag { get; } = nameof(SteamAppraiserAsync);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(SteamGameInfo);

        /// <inheritdoc />
        public override string RatingName { get; } = "Rating based on discount value";


        public SteamAppraiserAsync()
        {
        }

        #region GameAppraiserAsync Overriden Methods

        public override async Task<bool> GetRatings(ISourceBlock<BasicInfo> entitiesInfoQueue,
            ITargetBlock<RatingDataContainer> entitiesRatingQueue, bool outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                BasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();
                var gameInfo = (SteamGameInfo) entityInfo;

                var resultInfo = new RatingDataContainer(entityInfo, gameInfo.VoteCount);
                await entitiesRatingQueue.SendAsync(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Appraised {resultInfo} by {Tag}.");
                }
            }
            return true;
        }

        #endregion
    }
}
