using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Concrete crawler for The Movie Database service.
    /// </summary>
    public class CCrawlerTMDB : CCrawler
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CCrawlerTMDB>();

        /// <summary>
        /// Key to get access to TMDB service.
        /// </summary>
        private readonly String _APIKey;

        /// <summary>
        /// Requests number which are sent without timeout.
        /// </summary>
        private readonly Int32 _requestsPerTime;

        /// <summary>
        /// Value of good response.
        /// </summary>
        private readonly String _goodStatusCode;

        /// <summary>
        /// Number of maximum attempts.
        /// </summary>
        private readonly Int32 _limitAttempts;

        /// <summary>
        /// Timeout between each request.
        /// </summary>
        private readonly Int32 _millisecondsTimeout;

        /// <summary>
        /// REST client to send search requests.
        /// </summary>
        private readonly RestClient _restSearchClient;

        /// <summary>
        /// REST client for image configuration requests.
        /// </summary>
        private readonly RestClient _restImageConfigurationClient;

        /// <summary>
        /// Counter of attempts. Reset after successful response processing.
        /// </summary>
        private Int32 _actualRequestCounter;

        /// <summary>
        /// Stores service configuration.
        /// </summary>
        public static CServiceConfigurationTMDB ServiceConfigurationTMDB { get; private set; }


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="APIKey">Key to get access to TMDB service.</param>
        /// <param name="searchUrl">Url to send search request.</param>
        /// <param name="configurationUrl">Url for configuration request.</param>
        /// <param name="requestsPerTime">Requests number which are sent without timeout.</param>
        /// <param name="goodStatusCode">Value of good response.</param>
        /// <param name="limitAttempts">
        /// Number of maximum attempts to get response from TMDB service.
        /// If this limit is attained exception <see cref="InvalidOperationException" /> will
        /// be thrown by <see cref="GetResponse" /> method.
        /// </param>
        /// <param name="millisecondsTimeout">Timeout between each request.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="APIKey">APIKey</paramref> or <param name="searchUrl">searchUrl</param>
        /// or <param name="goodStatusCode">goodStatusCode</param> is <c>null</c> or presents empty
        /// strings.
        /// </exception>
        public CCrawlerTMDB(String APIKey, String searchUrl, String configurationUrl, 
            Int32 requestsPerTime, String goodStatusCode, Int32 limitAttempts,
            Int32 millisecondsTimeout)
        {
            searchUrl.ThrowIfNullOrEmpty(nameof(searchUrl));

            _APIKey = APIKey.ThrowIfNullOrEmpty(nameof(APIKey));
            _requestsPerTime = requestsPerTime;
            _goodStatusCode = goodStatusCode.ThrowIfNullOrEmpty(nameof(goodStatusCode));
            _limitAttempts = limitAttempts;
            _millisecondsTimeout = millisecondsTimeout;

            _restSearchClient = new RestClient(searchUrl);
            _restImageConfigurationClient = new RestClient(configurationUrl);
        }

        #region CCrawler Overridden Methods

        /// <inheritdoc />
        public override List<CBasicInfo> GetResponse(List<String> entities, Boolean outputResults)
        {
            _actualRequestCounter = 0;
            ServiceConfigurationTMDB = GetServiceConfiguration(outputResults);

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<CBasicInfo>();
            foreach (String movie in entities)
            {
                JObject response = TryGetResponse(movie);
                ++_actualRequestCounter;

                if (_actualRequestCounter == _requestsPerTime)
                {
                    _actualRequestCounter = 0;
                    SHelperMethods.Sleep(_millisecondsTimeout);
                }

                if (!response["results"].HasValues)
                {
                    s_logger.Warn($"{movie} wasn't processed.");
                    SGlobalMessageHandler.OutputMessage($"{movie} wasn't processed.");
                    continue;
                }

                // Get first search result from response and ignore all the rest.
                JToken result = response["results"][0];
                if (outputResults)
                {
                    SGlobalMessageHandler.OutputMessage(result.ToString());
                }

                // JToken.ToObject is a helper method that uses JsonSerializer internally.
                var searchResult = result.ToObject<CMovieTMDBInfo>();
                searchResults.Add(searchResult);
            }
            return searchResults.ToList();
        }

        #endregion

        /// <summary>
        /// Sends request to get configuration of the service.
        /// </summary>
        /// <returns>Response in REST format.</returns>
        private IRestResponse SendGetConfigurationQuery()
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("api_key", _APIKey, ParameterType.QueryString);
            IRestResponse response = _restImageConfigurationClient.Execute(request);
            return response;
        }

        /// <summary>
        /// Gets service configuration.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Transformed configuration of the service.</returns>
        private CServiceConfigurationTMDB GetServiceConfiguration(Boolean outputResults)
        {
            JObject response = GetResponseResult(SendGetConfigurationQuery());

            if (!response["images"].HasValues)
            {
                s_logger.Warn("Image configuration cannot be obtained.");
                SGlobalMessageHandler.OutputMessage("Image configuration cannot be obtained.");
            }

            // Get first search result from response and ignore all the rest.
            JToken result = response["images"];
            if (outputResults)
            {
                SGlobalMessageHandler.OutputMessage(result.ToString());
            }

            // JToken.ToObject is a helper method that uses JsonSerializer internally.
            return result.ToObject<CServiceConfigurationTMDB>();
        }

        /// <summary>
        /// Sends search request to service.
        /// </summary>
        /// <param name="entityName">Entity name to search.</param>
        /// <returns>Raw server response.</returns>
        private IRestResponse SendSearchQuery(String entityName)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("api_key", _APIKey, ParameterType.QueryString);
            request.AddParameter("query", entityName, ParameterType.QueryString);

            // Get only first page.
            request.AddParameter("page", "1", ParameterType.QueryString);
            IRestResponse response = _restSearchClient.Execute(request);
            return response;
        }

        /// <summary>
        /// Tries to get a response from the service several times.
        /// </summary>
        /// <param name="entityName">Entity name for request.</param>
        /// <returns>Parsed response content.</returns>
        /// <remarks>
        /// Attempts number set in the constructor. You can change this value in config file.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Reached the limit of attempts.</exception>
        private JObject TryGetResponse(String entityName)
        {
            Int32 numberOfAttempts = 1;

            JObject response = GetResponseResult(SendSearchQuery(entityName));
            while (!(response["status_code"] is null) &&
                   response["status_code"].ToString() != _goodStatusCode)
            {
                _actualRequestCounter = 0;
                if (numberOfAttempts > _limitAttempts)
                {
                    var ex = new InvalidOperationException("Couldn't get good response from TMDB.");
                    s_logger.Error(ex, $"TMDB was unavailable for {_limitAttempts} attempts.");
                    throw ex;
                }

                // Increase timeout between attempts.
                SHelperMethods.Sleep(numberOfAttempts * _millisecondsTimeout);
                ++numberOfAttempts;
                response = GetResponseResult(SendSearchQuery(entityName));
            }
            return response;
        }
    }
}
