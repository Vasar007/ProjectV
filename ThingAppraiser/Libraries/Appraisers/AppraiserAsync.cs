using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    public abstract class AppraiserAsync : AppraiserBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override string Tag { get; } = nameof(AppraiserAsync);

        #endregion


        protected AppraiserAsync()
        {
        }

        public virtual async Task<bool> GetRatings(BufferBlock<BasicInfo> entitiesInfoQueue,
            BufferBlock<RatingDataContainer> entitiesRatingQueue, bool outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                BasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();

                var resultInfo = new RatingDataContainer(entityInfo, entityInfo.VoteAverage);
                await entitiesRatingQueue.SendAsync(resultInfo);

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }
            return true;
        }
    }
}
