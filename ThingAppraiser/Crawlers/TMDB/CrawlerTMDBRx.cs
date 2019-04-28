using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public class CCrawlerTMDBRx : CCrawlerRx
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CCrawlerTMDBRx>();

        private readonly String _APIKey;

        private readonly Int32 _requestsPerTime;

        private readonly String _goodStatusCode;

        private readonly Int32 _limitAttempts;

        private readonly Int32 _millisecondsTimeout;

        private readonly RestClient _restSearchClient;

        private readonly RestClient _restImageConfigurationClient;

        private readonly ConcurrentDictionary<CBasicInfo, Byte> _searchResults;

        /// <inheritdoc />
        public override String Tag => "CrawlerTMDBRx";

        /// <inheritdoc />
        public override Type TypeID => typeof(CMovieTMDBInfo);


        public CCrawlerTMDBRx(String APIKey, String searchUrl, String configurationUrl, 
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

            _searchResults = new ConcurrentDictionary<CBasicInfo, Byte>();
        }

        #region CCrawlerSync Overridden Methods

        public override async Task<CBasicInfo> FindResponse(String entity, Boolean outputResults)
        {
            if (SServiceConfigurationTMDB.Configuration is null)
            {
                SServiceConfigurationTMDB.Configuration = 
                    await GetServiceConfiguration(outputResults);
            }

            JObject response = await TryGetResponse(entity);

            if (!response["results"].HasValues)
            {
                s_logger.Warn($"{entity} wasn't processed.");
                SGlobalMessageHandler.OutputMessage($"{entity} wasn't processed.");
                return null;
            }

            // Get first search result from response and ignore all the rest.
            JToken result = response["results"][0];
            if (outputResults)
            {
                SGlobalMessageHandler.OutputMessage(result.ToString());
            }

            // JToken.ToObject is a helper method that uses JsonSerializer internally.
            var searchResult = result.ToObject<CMovieTMDBInfo>();

            if (_searchResults.TryAdd(searchResult, default(Byte)))
            {
                return searchResult;
            }
            return null;
        }

        #endregion

        private async Task<IRestResponse> SendGetConfigurationQuery()
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("api_key", _APIKey, ParameterType.QueryString);
            IRestResponse response = await _restImageConfigurationClient.ExecuteTaskAsync(request);
            return response;
        }

        private async Task<CServiceConfigurationInfoTMDB> GetServiceConfiguration(
            Boolean outputResults)
        {
            JObject response = GetResponseResult(await SendGetConfigurationQuery());

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
            return result.ToObject<CServiceConfigurationInfoTMDB>();
        }

        private async Task<IRestResponse> SendSearchQuery(String entityName)
        {
            var request = new RestRequest(Method.GET);
            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("api_key", _APIKey, ParameterType.QueryString);
            request.AddParameter("query", entityName, ParameterType.QueryString);

            // Get only first page.
            request.AddParameter("page", "1", ParameterType.QueryString);
            IRestResponse response = await _restSearchClient.ExecuteTaskAsync(request);
            return response;
        }

        private async Task<JObject> TryGetResponse(String entityName)
        {
            Int32 numberOfAttempts = 1;

            JObject response = GetResponseResult(await SendSearchQuery(entityName));
            while (!(response["status_code"] is null) &&
                   response["status_code"].ToString() != _goodStatusCode)
            {
                if (numberOfAttempts > _limitAttempts)
                {
                    var ex = new InvalidOperationException("Couldn't get good response from TMDB.");
                    s_logger.Error(ex, $"TMDB was unavailable for {_limitAttempts} attempts.");
                    throw ex;
                }

                // Increase timeout between attempts.
                await Task.Delay(numberOfAttempts * _millisecondsTimeout);
                ++numberOfAttempts;
                response = GetResponseResult(await SendSearchQuery(entityName));
            }
            return response;
        }
    }
}