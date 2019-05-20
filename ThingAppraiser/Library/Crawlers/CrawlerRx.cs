using System;
using System.Threading.Tasks;
using ThingAppraiser.Data;

namespace ThingAppraiser.Crawlers
{
    public abstract class CrawlerRx : CrawlerBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override String Tag { get; } = "CrawlerRx";

        #endregion


        protected CrawlerRx()
        {
        }

        public abstract Task<BasicInfo> FindResponse(String entity, Boolean outputResults);
    }
}
