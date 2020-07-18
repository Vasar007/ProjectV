using System.Collections.Generic;
using ProjectV.Models.Data;

namespace ProjectV.Crawlers
{
    public interface ICrawlerAsync : ICrawlerBase
    {
        IAsyncEnumerable<BasicInfo> GetResponse(string entitiyName, bool outputResults);
    }
}
