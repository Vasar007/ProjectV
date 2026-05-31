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
    /// Per the <c>create-tests</c> scenario rules, scenario tests must use
    /// concrete stubs (named <c>Stub{DependencyName}</c>) rather than
    /// NSubstitute mocks or <c>Test*Builder</c> helpers for types they own.
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
            return Task.FromResult(
                new Result<TokenResponse, ErrorResponse>(new ErrorResponse()));
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
            return Task.FromResult(
                new Result<ProcessingResponse, ErrorResponse>(new ErrorResponse()));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Stub holds no resources.
        }
    }
}
