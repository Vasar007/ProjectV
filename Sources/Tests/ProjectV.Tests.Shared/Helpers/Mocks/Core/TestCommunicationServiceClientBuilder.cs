using System.Threading;
using Acolyte.Assertions;
using Acolyte.Common;
using AutoFixture;
using ProjectV.Core.Services.Clients;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Core
{
    /// <summary>
    /// Builder for <see cref="ICommunicationServiceClient" /> test doubles
    /// backed by AutoFixture + NSubstitute.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The production concrete <c>CommunicationServiceClient</c> constructs
    /// its <see cref="System.Net.Http.HttpClient" /> and inner token clients
    /// in its constructor, which makes it expensive to wire up for a plain
    /// unit test. The interface seam
    /// <see cref="ICommunicationServiceClient" /> is the natural test
    /// substitution target — downstream web-service orchestration tests
    /// consume the same shape.
    /// </para>
    /// <para>
    /// Tests that need to exercise the production concrete (e.g. HTTP
    /// pipeline tests in <c>ProjectV.Core.Tests</c>) construct it directly
    /// with a <see cref="System.Net.Http.IHttpClientFactory" /> backed by a
    /// <c>FakeHttpMessageHandler</c>; they do not use this builder.
    /// </para>
    /// </remarks>
    public sealed class TestCommunicationServiceClientBuilder
    {
        private readonly IFixture _fixture;

        private Result<TokenResponse, ErrorResponse>? _loginResponse;
        private Result<ProcessingResponse, ErrorResponse>? _startJobResponse;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestCommunicationServiceClientBuilder" /> class. No
        /// behavior is configured until one of the <c>With*</c> methods is
        /// called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestCommunicationServiceClientBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="ICommunicationServiceClient" /> substitute with no
        /// configured behavior.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static ICommunicationServiceClient CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestCommunicationServiceClientBuilder(fixture).Build();
        }

        /// <summary>
        /// Configures the substitute to return the supplied successful
        /// <see cref="TokenResponse" /> for every call to
        /// <see cref="ICommunicationServiceClient.LoginAsync" />.
        /// </summary>
        /// <param name="response">Token response to wrap. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCommunicationServiceClientBuilder WithLoginResponse(TokenResponse response)
        {
            response.ThrowIfNull(nameof(response));

            _loginResponse = Result.Ok<TokenResponse>(response);
            return this;
        }

        /// <summary>
        /// Configures the substitute to return the supplied
        /// <see cref="ErrorResponse" /> as a login failure for every call to
        /// <see cref="ICommunicationServiceClient.LoginAsync" />.
        /// </summary>
        /// <param name="error">Error response to wrap. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCommunicationServiceClientBuilder WithLoginError(ErrorResponse error)
        {
            error.ThrowIfNull(nameof(error));

            _loginResponse = Result.Error<ErrorResponse>(error);
            return this;
        }

        /// <summary>
        /// Configures the substitute to return the supplied successful
        /// <see cref="ProcessingResponse" /> for every call to
        /// <see cref="ICommunicationServiceClient.StartJobAsync" />.
        /// </summary>
        /// <param name="response">
        /// Processing response to wrap. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCommunicationServiceClientBuilder WithStartJobResponse(
            ProcessingResponse response)
        {
            response.ThrowIfNull(nameof(response));

            _startJobResponse = Result.Ok<ProcessingResponse>(response);
            return this;
        }

        /// <summary>
        /// Configures the substitute to return the supplied
        /// <see cref="ErrorResponse" /> as a job-start failure for every
        /// call to <see cref="ICommunicationServiceClient.StartJobAsync" />.
        /// </summary>
        /// <param name="error">Error response to wrap. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCommunicationServiceClientBuilder WithStartJobError(ErrorResponse error)
        {
            error.ThrowIfNull(nameof(error));

            _startJobResponse = Result.Error<ErrorResponse>(error);
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ICommunicationServiceClient" /> substitute.
        /// If no <c>With*</c> method has been called, the substitute returns
        /// whatever AutoFixture / NSubstitute would by default.
        /// </summary>
        public ICommunicationServiceClient Build()
        {
            var substitute = _fixture.Create<ICommunicationServiceClient>();

            if (_loginResponse is { } loginResponse)
            {
                substitute
                    .LoginAsync(Arg.Any<LoginRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(loginResponse));
            }

            if (_startJobResponse is { } startJobResponse)
            {
                substitute
                    .StartJobAsync(Arg.Any<StartJobParamsRequest>(), Arg.Any<CancellationToken>())
                    .Returns(Task.FromResult(startJobResponse));
            }

            return substitute;
        }
    }
}
