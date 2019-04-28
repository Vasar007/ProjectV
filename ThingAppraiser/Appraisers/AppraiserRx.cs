using System;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;

namespace ThingAppraiser.Appraisers
{
    public abstract class CAppraiserRx : ITagable, ITypeID
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual String Tag => "AppraiserRx";

        #endregion

        #region ITypeID Implementation

        public virtual Type TypeID => typeof(CBasicInfo);

        #endregion


        public CAppraiserRx()
        {
        }

        public virtual CRatingDataContainer GetRatings(CBasicInfo entityInfo,
            Boolean outputResults)
        {
            var resultInfo = new CRatingDataContainer(entityInfo, entityInfo.VoteAverage);

            if (outputResults)
            {
                SGlobalMessageHandler.OutputMessage(resultInfo.ToString());
            }

            return resultInfo;
        }
    }
}
