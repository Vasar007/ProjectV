using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;
using ProjectV.Logging;
using ProjectV.Models.Configuration;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Configuration
{
    public sealed class ConfigurationReceiver : IConfigurationReceiver
    {
        private static readonly ILogger _logger =
           LoggerFactory.CreateLoggerFor<ConfigurationReceiver>();

        private readonly ProjectVServiceOptions _serviceOptions;

        private readonly HttpClient _client;
        private readonly bool _continueOnCapturedContext;

        private string BaseAddress => _serviceOptions.RestApi.ConfigurationServiceBaseAddress;
        private string ApiUrl => _serviceOptions.RestApi.ConfigurationServiceApiUrl;


        public ConfigurationReceiver(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serivceSettings)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _serviceOptions = serivceSettings.Value.ThrowIfNull(nameof(serivceSettings));

            _client = httpClientFactory.CreateClientWithOptions(BaseAddress, _serviceOptions);
            _continueOnCapturedContext = false;
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _client.DisposeClient(_serviceOptions);

            _disposed = true;
        }

        #endregion

        #region IConfigurationReceiverAsync Implementation

        public async Task<Result<StartJobDataResponce, ErrorResponse>> ReceiveConfigForRequestAsync(
            StartJobParamsRequest jobParams)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            _logger.Info("Sending config request and trying to receive response.");

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                .AsJson(jobParams.Requirements);

            var result = await _client.SendAndReadAsync<ConfigurationXml>(
                    request, _logger, _continueOnCapturedContext, CancellationToken.None
                )
                .ConfigureAwait(_continueOnCapturedContext);

            if (result.IsSuccess && result.Ok is not null)
            {
                _logger.Info("Received successful config response.");
                var config = result.Ok;

                var requestData = new StartJobDataResponce
                {
                    ThingNames = jobParams.ThingNames,
                    ConfigurationXml = config
                };
                return Result.Ok(requestData);
            }

            _logger.Error("Received bad config response.");

            if (!result.IsSuccess)
            {
                return Result.Error(result.Error!);
            }

            // In case service returns null config and no error.
            var error = new ErrorResponse
            {
                Success = false,
                ErrorCode = "C01",
                ErrorMessage = "Config request failed."
            };
            return Result.Error(error);
        }

        #endregion
    }
}
