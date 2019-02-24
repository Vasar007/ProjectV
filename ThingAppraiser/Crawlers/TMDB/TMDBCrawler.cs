using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ThingAppraiser.Crawlers
{
    public class TMDBCrawler : Crawler
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string _APIKey;
        private readonly string _searchUrl;
        private readonly int _requestsPerTime;
        private readonly string _goodStatusCode;
        private readonly int _limitAttempts;
        private readonly int _millisecondsTimeout;

        protected override RestClient Client { get; }

        public TMDBCrawler(string APIKey, string searchUrl, int requestsPerTime = 30,
            string goodStatusCode = "200", int limitAttempts = 10, int millisecondsTimeout = 1000)
        {
            _APIKey = APIKey;
            _searchUrl = searchUrl;
            _requestsPerTime = requestsPerTime;
            _goodStatusCode = goodStatusCode;
            _limitAttempts = limitAttempts;
            _millisecondsTimeout = millisecondsTimeout;

            Client = new RestClient(_searchUrl);
        }

        private void Sleep(int millisecondsTimeout = 1000)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        private JObject GetResponse(string entityName)
        {
            int numberOfAttempts = 1;

            var response = GetSearchResult(SendSearchQuery(entityName));
            while (!(response["status_code"] is null) &&
                   response["status_code"].ToString() != _goodStatusCode)
            {
                if (numberOfAttempts > _limitAttempts)
                {
                    var ex = new InvalidOperationException("Couldn't get good response from TMDB.");
                    _logger.Error(ex, $"TMDB was unavailable for {_limitAttempts} attempts.");
                    throw ex;
                }

                // Increase timeout between attempts.
                Sleep(numberOfAttempts * _millisecondsTimeout);
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

        public override List<Data.DataHandler> GetResponse(List<string> entities,
            bool ouput = false)
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
