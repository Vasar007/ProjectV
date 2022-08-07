using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
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
        private HttpClientOptions HcOptions => _serviceOptions.HttpClient;


        public ConfigurationReceiver(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serviceSettings)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _serviceOptions = serviceSettings.GetCheckedValue();

            _client = httpClientFactory.CreateClientWithOptions(BaseAddress, HcOptions);
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

            _client.DisposeClient(HcOptions);

            _disposed = true;
        }

        #endregion

        #region IConfigurationReceiverAsync Implementation

        public async Task<Result<StartJobDataResponce, ErrorResponse>> ReceiveConfigForRequestAsync(
            StartJobParamsRequest jobParams, CancellationToken cancellationToken = default)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            _logger.Info("Sending config request and trying to receive response.");

            var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                .AsJson(jobParams.Requirements);

            try
            {
                var result = await _client.SendAndReadAsync<ConfigurationXml>(
                  request, _logger, _continueOnCapturedContext, cancellationToken
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to receive config response.");

                var error = new ErrorResponse
                {
                    Success = false,
                    ErrorCode = "C02",
                    ErrorMessage = ex.Message
                };
                return Result.Error(error);
            }
        }

        #endregion
    }
}
