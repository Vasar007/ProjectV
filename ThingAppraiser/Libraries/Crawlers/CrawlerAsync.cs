using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    public abstract class CrawlerAsync : CrawlerBase
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public override string Tag { get; } = nameof(CrawlerAsync);

        #endregion


        protected CrawlerAsync()
        {
        }

        public abstract Task<bool> GetResponse(BufferBlock<string> entitiesQueue,
            BufferBlock<BasicInfo> responsesQueue, bool outputResults);
    }
}
