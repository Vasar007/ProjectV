using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ThingAppraiser.Crawlers
{
    public abstract class Crawler
    {
        protected abstract RestClient Client { get; }

        protected virtual JObject GetSearchResult(IRestResponse response)
        {
            var parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }

        protected abstract IRestResponse SendSearchQuery(string entityName);

        public abstract List<Data.DataHandler> GetResponse(List<string> entities, bool ouput = false);
    }
}
