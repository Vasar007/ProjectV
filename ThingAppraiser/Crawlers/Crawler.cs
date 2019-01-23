using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ThingAppraiser.Crawlers
{
    public abstract class Crawler<T> where T : Data.DataHandler
    {
        public virtual IRestResponse SendSearchQuery(string entityName)
        {
            var client = new RestClient(GetSearchQueryString(entityName));
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public virtual JObject GetSearchResult(IRestResponse response)
        {
            var parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }

        public abstract string GetSearchQueryString(string entityName);

        public abstract List<T> GetData(string[] entities, bool ouput = false);
    }
}
