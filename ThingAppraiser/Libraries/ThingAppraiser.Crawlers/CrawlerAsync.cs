using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    public interface ICrawlerAsync : ICrawlerBase
    {
        Task<bool> GetResponse(ISourceBlock<string> entitiesQueue,
            ITargetBlock<BasicInfo> responsesQueue, bool outputResults);
    }
}
