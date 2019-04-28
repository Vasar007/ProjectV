using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThingAppraiser.Data;

namespace ThingAppraiser.Crawlers
{
    public abstract class CCrawlerRx : ITagable, ITypeID
    {
        #region ITagable Implementation

        /// <inheritdoc />
        public virtual String Tag => "CrawlerRx";

        #endregion

        #region ITypeID Implementation

        public virtual Type TypeID => typeof(CBasicInfo);
        
        #endregion


        public CCrawlerRx()
        {
        }

        public abstract Task<CBasicInfo> FindResponse(String entity, Boolean outputResults);

        protected virtual JObject GetResponseResult(IRestResponse response)
        {
            JObject parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }
    }
}
