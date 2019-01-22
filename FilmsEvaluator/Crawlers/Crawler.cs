using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace FilmsEvaluator.Crawlers
{
    public abstract class Crawler<T> where T : Movie
    {
        public virtual IRestResponse SendSearchQuery(string filmName)
        {
            var client = new RestClient(GetSearchQueryString(filmName));
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            return response;
        }

        public virtual JObject GetSearchResult(IRestResponse response)
        {
            var parsed_json = JObject.Parse(response.Content);
            return parsed_json;
        }

        public abstract string GetSearchQueryString(string filmName);

        public abstract List<T> GetFilmsInfo(string[] films, bool ouput = false);
    }
}
