using System.Threading;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Core.Services.Clients;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram
{
    /// <summary>
    /// Scenario-test stub for <see cref="ICommunicationServiceClient" />.
    /// Returns deterministic no-op / default values for every interface
    /// member. The scenario flows exercised by the polling and webhook
    /// tests do not reach the communication-service client at all; this
    /// stub satisfies the DI graph without requiring a real HTTP stack or
    /// NSubstitute.
    /// </summary>
    /// <remarks>
    /// Scenario tests compose a real dependency graph using concrete stubs
    /// instead of interface mocks, so external/leaf dependencies are replaced
    /// with deterministic stub implementations rather than NSubstitute mocks
    /// or <c>Test*Builder</c> helpers.
    /// </remarks>
    public sealed class StubCommunicationServiceClient : ICommunicationServiceClient
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="StubCommunicationServiceClient" /> class.
        /// </summary>
        public StubCommunicationServiceClient()
        {
        }

        /// <inheritdoc />
        /// <remarks>
        /// Returns a default (empty) error result as a deterministic no-op.
        /// The scenario flows under test do not invoke this method.
        /// </remarks>
        public Task<Result<TokenResponse, ErrorResponse>> LoginAsync(
            LoginRequest login,
            CancellationToken cancellationToken = default)
        {
            Result<TokenResponse, ErrorResponse> result = Result.Error<ErrorResponse>(new ErrorResponse());
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Returns a default (empty) error result as a deterministic no-op.
        /// The scenario flows under test do not invoke this method.
        /// </remarks>
        public Task<Result<ProcessingResponse, ErrorResponse>> StartJobAsync(
            StartJobParamsRequest jobParams,
            CancellationToken cancellationToken = default)
        {
            Result<ProcessingResponse, ErrorResponse> result = Result.Error<ErrorResponse>(new ErrorResponse());
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Stub holds no resources.
        }
    }
}
