using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ThingAppraiser.Crawlers
{
    public abstract class Crawler
    {
        protected abstract RestClient Client { get; }

        public virtual JObject GetSearchResult(IRestResponse response)
        {
            var parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }

        public abstract IRestResponse SendSearchQuery(string entityName);

        public abstract List<Data.DataHandler> GetData(string[] entities, bool ouput = false);
    }
}
