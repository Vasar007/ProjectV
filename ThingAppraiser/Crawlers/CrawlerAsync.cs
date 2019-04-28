using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThingAppraiser.Data;

namespace ThingAppraiser.Crawlers
{
    public abstract class CCrawlerAsync : ITagable, ITypeID
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual String Tag => "CrawlerAsync";

        #endregion

        #region ITypeID Implementation

        public virtual Type TypeID => typeof(CBasicInfo);
        
        #endregion


        public CCrawlerAsync()
        {
        }

        public abstract Task<Boolean> GetResponse(BufferBlock<String> entitiesQueue,
            BufferBlock<CBasicInfo> responsesQueue, Boolean outputResults);

        protected virtual JObject GetResponseResult(IRestResponse response)
        {
            JObject parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }
    }
}
