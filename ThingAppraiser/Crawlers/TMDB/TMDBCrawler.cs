using System;
using System.Collections.Generic;
using RestSharp;

namespace ThingAppraiser.Crawlers
{
    public class TMDBCrawler : Crawler
    {
        private const string _APIKey = "";
        private const string _searchUrl = "https://api.themoviedb.org/3/search/movie";

        protected override RestClient Client { get; } = new RestClient(_searchUrl);

        public override IRestResponse SendSearchQuery(string entityName)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("api_key", _APIKey, ParameterType.QueryString);
            request.AddParameter("query", entityName, ParameterType.QueryString);
            request.AddParameter("page", "1", ParameterType.QueryString); // Get only first page.
            IRestResponse response = Client.Execute(request);
            return response;
        }

        public override List<Data.DataHandler> GetData(string[] movies, bool ouput = false)
        {
            var searchResults = new List<Data.DataHandler>();
            foreach (var movie in movies)
            {
                var response = GetSearchResult(SendSearchQuery(movie));

                if (!response["results"].HasValues) continue;

                // Get first search result from response.
                var result = response["results"][0];
                if (ouput)
                {
                    Console.WriteLine(result);
                }
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                var searchResult = result.ToObject<Data.TMDBMovie>();
                searchResults.Add(searchResult);
            }
            return searchResults;
        }
    }
}
