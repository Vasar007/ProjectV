using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ThingAppraiser.Crawlers
{
    public class TMDBCrawler : Crawler
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const string _searchUrl = "https://api.themoviedb.org/3/search/movie";
        private const int _requestsPerTime = 30;
        private readonly string _APIKey;

        protected override RestClient Client { get; } = new RestClient(_searchUrl);

        public TMDBCrawler()
        {
            // Load API key from credentials file.
            var json = JObject.Parse(File.ReadAllText("credentials.json"));
            _APIKey = json["TMDBAPIKey"].ToString();
        }

        private void Sleep(int millisecondsTimeout = 1000)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        private JObject GetResponse(string entityName)
        {
            // TODO: encapsulate this const variables into fields and set them in constructor.
            const string goodStatusCode = "200";
            const int limitAttempts = 10;
            const int millisecondsTimeout = 1000;
            int numberOfAttempts = 1;

            var response = GetSearchResult(SendSearchQuery(entityName));
            while (!(response["status_code"] is null) &&
                   response["status_code"].ToString() != goodStatusCode)
            {
                if (numberOfAttempts > limitAttempts)
                {
                    _logger.Error("Couldn't get good response from TMDB.");
                    throw new InvalidOperationException("Couldn't get good response from TMDB.");
                }
                Sleep(numberOfAttempts * millisecondsTimeout);
                ++numberOfAttempts;
                response = GetSearchResult(SendSearchQuery(entityName));
            }
            return response;
        }

        protected override IRestResponse SendSearchQuery(string entityName)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("api_key", _APIKey, ParameterType.QueryString);
            request.AddParameter("query", entityName, ParameterType.QueryString);
            request.AddParameter("page", "1", ParameterType.QueryString); // Get only first page.
            IRestResponse response = Client.Execute(request);
            return response;
        }

        public override List<Data.DataHandler> GetData(List<string> entities, bool ouput = false)
        {
            var searchResults = new List<Data.DataHandler>();
            foreach (var movie in entities)
            {
                var response = GetResponse(movie);

                if (!response["results"].HasValues)
                {
                    _logger.Warn($"{movie} wasn't processed.");
                    Core.Shell.OutputMessage($"{movie} wasn't processed.");
                    continue;
                }
                // Get first search result from response.
                var result = response["results"][0];
                if (ouput)
                {
                    Core.Shell.OutputMessage(result.ToString());
                }
                // JToken.ToObject is a helper method that uses JsonSerializer internally
                var searchResult = result.ToObject<Data.TMDBMovie>();
                searchResults.Add(searchResult);
            }
            return searchResults;
        }
    }
}
