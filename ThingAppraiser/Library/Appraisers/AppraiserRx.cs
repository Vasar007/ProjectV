using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class AppraiserRx : AppraiserBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override string Tag { get; } = "AppraiserRx";

        #endregion


        public AppraiserRx()
        {
        }

        public virtual RatingDataContainer GetRatings(BasicInfo entityInfo, bool outputResults)
        {
            var resultInfo = new RatingDataContainer(entityInfo, entityInfo.VoteAverage);

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage(resultInfo.ToString());
            }

            return resultInfo;
        }
    }
}
