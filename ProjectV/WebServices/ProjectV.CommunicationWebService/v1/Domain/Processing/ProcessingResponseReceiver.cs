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
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Processing
{
    public sealed class ProcessingResponseReceiver : IProcessingResponseReceiver
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ProcessingResponseReceiver>();

        private readonly ProjectVServiceOptions _serviceOptions;

        private readonly HttpClient _client;
        private readonly bool _continueOnCapturedContext;

        private string BaseAddress => _serviceOptions.RestApi.ProcessingServiceBaseAddress;
        private string ApiUrl => _serviceOptions.RestApi.ProcessingServiceApiUrl;
        private HttpClientOptions HcOptions => _serviceOptions.HttpClient;


        public ProcessingResponseReceiver(
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

        #region IProcessingResponseReceiverAsync Implementation

        public async Task<Result<ProcessingResponse, ErrorResponse>> ReceiveProcessingResponseAsync(
            StartJobDataResponce jobData, CancellationToken cancellationToken = default)
        {
            jobData.ThrowIfNull(nameof(jobData));

            _logger.Info("Sending processing request and trying to receive response.");

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl)
                    .AsJson(jobData);

                return await _client.SendAndReadAsync<ProcessingResponse>(
                        request, _logger, _continueOnCapturedContext, cancellationToken
                    )
                    .ConfigureAwait(_continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to receive processing response.");

                var error = new ErrorResponse
                {
                    Success = false,
                    ErrorCode = "P01",
                    ErrorMessage = ex.Message
                };
                return Result.Error(error);
            }
        }

        #endregion
    }
}
