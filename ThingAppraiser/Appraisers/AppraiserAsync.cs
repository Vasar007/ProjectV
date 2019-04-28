using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class CAppraiserAsync : ITagable, ITypeID
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual String Tag => "AppraiserAsync";

        #endregion

        #region ITypeID Implementation

        public virtual Type TypeID => typeof(CBasicInfo);

        #endregion

        public CAppraiserAsync()
        {
        }

        public virtual async Task<Boolean> GetRatings(BufferBlock<CBasicInfo> entitiesInfoQueue,
            BufferBlock<CRatingDataContainer> entitiesRatingQueue, Boolean outputResults)
        {
            while (await entitiesInfoQueue.OutputAvailableAsync())
            {
                CBasicInfo entityInfo = await entitiesInfoQueue.ReceiveAsync();

                var resultInfo = new CRatingDataContainer(entityInfo, entityInfo.VoteAverage);
                await entitiesRatingQueue.SendAsync(resultInfo);

                if (outputResults)
                {
                    SGlobalMessageHandler.OutputMessage(resultInfo.ToString());
                }
            }
            return true;
        }
    }
}
