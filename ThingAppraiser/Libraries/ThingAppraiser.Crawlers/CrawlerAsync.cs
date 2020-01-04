using System.Collections.Generic;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    public interface ICrawlerAsync : ICrawlerBase
    {
        IAsyncEnumerable<BasicInfo> GetResponse(string entitiyName, bool outputResults);
    }
}
