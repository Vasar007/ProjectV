using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using Newtonsoft.Json;
using NSubstitute;
using ProjectV.Configuration.Options;
using ProjectV.Core.Services.Clients;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;
using ProjectV.Tests.Shared.Helpers.Http;
using Xunit;

namespace ProjectV.Core.Tests.Net
{
    /// <summary>
    /// Unit tests for <see cref="CommunicationServiceClient.LoginAsync" />.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Substitutes <see cref="IHttpClientFactory" /> via NSubstitute and
    /// returns a real <see cref="HttpClient" /> backed by an in-test
    /// <see cref="FakeHttpMessageHandler" /> (DelegatingHandler subclass) — the
    /// anti-pattern of substituting <see cref="HttpMessageHandler" /> with
    /// NSubstitute is avoided per 02-RESEARCH.md "Pitfall 6: NSubstitute cannot
    /// mock protected SendAsync".
    /// </para>
    /// <para>
    /// The plan called for a "throws AuthFailureException on 401" test; the
    /// production code returns <c>Result.Error&lt;ErrorResponse&gt;</c> on
    /// non-success status codes via
    /// <see cref="ProjectV.Core.Net.Http.HttpResponseMessageExtensions" />
    /// — it does NOT throw. Test was adjusted to match observed behaviour
    /// (recorded as deviation in <c>02-05-SUMMARY.md</c> Deviations §2).
    /// </para>
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class CommunicationServiceClientTests
    {
        private const string TestBaseAddress = "http://localhost:8000/";
        private const string TestLoginApiUrl = "/api/v1/Users/Login";
        private const string TestRequestApiUrl = "/api/v1/Requests";

        public CommunicationServiceClientTests()
        {
        }

        [Fact]
        public async Task LoginAsync_WithSuccessfulResponse_ReturnsTokenResponse()
        {
            // Arrange.
            var token = new TokenResponse
            {
                AccessToken = new AccessTokenData(
                    Token: "access-token-jwt",
                    ExpiryDateUtc: DateTime.UtcNow.AddMinutes(15)
                ),
                RefreshToken = new RefreshTokenData(
                    Token: "refresh-token-jwt",
                    ExpiryDateUtc: DateTime.UtcNow.AddDays(7)
                )
            };
            string body = JsonConvert.SerializeObject(token);

            using var handler = new FakeHttpMessageHandler(_ => CreateJsonResponse(HttpStatusCode.OK, body));
            using var sut = CreateSut(handler);

            // Act.
            var actualValue = await sut.LoginAsync(
                new LoginRequest { UserName = "user", Password = "pass" }
            );

            // Assert.
            actualValue.IsSuccess.Should().BeTrue();
            actualValue.Ok.Should().NotBeNull();
            actualValue.Ok!.AccessToken.Should().NotBeNull();
            actualValue.Ok!.AccessToken.Token.Should().Be("access-token-jwt");
            handler.CallCount.Should().Be(1);
        }

        [Fact]
        public async Task LoginAsync_With401Unauthorized_ReturnsErrorResponseWithCode401()
        {
            // Arrange.
            var errorPayload = new ErrorResponse
            {
                Success = false,
                ErrorCode = "401",
                ErrorMessage = "Invalid credentials."
            };
            string body = JsonConvert.SerializeObject(errorPayload);

            using var handler = new FakeHttpMessageHandler(
                _ => CreateJsonResponse(HttpStatusCode.Unauthorized, body));
            using var sut = CreateSut(handler);

            // Act.
            var actualValue = await sut.LoginAsync(
                new LoginRequest { UserName = "user", Password = "wrong" }
            );

            // Assert.
            actualValue.IsSuccess.Should().BeFalse();
            actualValue.Error.Should().NotBeNull();
            actualValue.Error!.ErrorCode.Should().Be("401");
            handler.CallCount.Should().Be(1);
        }

        [Fact]
        public async Task LoginAsync_WithNullLoginRequest_ThrowsArgumentNullException()
        {
            // Arrange.
            using var handler = new FakeHttpMessageHandler(_ => CreateJsonResponse(HttpStatusCode.OK, "{}"));
            using var sut = CreateSut(handler);

            // Act. / Assert.
            var act = async () => await sut.LoginAsync(login: null!);
            await act.Should()
                .ThrowAsync<ArgumentNullException>()
                .WithParameterName("login");
        }

        /// <summary>
        /// Constructs a real <see cref="CommunicationServiceClient" /> with a
        /// substituted <see cref="IHttpClientFactory" /> that returns an
        /// <see cref="HttpClient" /> backed by the supplied
        /// <see cref="FakeHttpMessageHandler" />.
        /// </summary>
        private static CommunicationServiceClient CreateSut(FakeHttpMessageHandler handler)
        {
            var httpClientFactory = Substitute.For<IHttpClientFactory>();
            // CreateClientWithOptions appends Configure* calls to a fresh HttpClient
            // returned by CreateClient — the handler must be passed at HttpClient
            // construction time (not via the factory).
            var client = new HttpClient(handler, disposeHandler: false);
            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(client);

            var serviceOptions = new ProjectVServiceOptions
            {
                RestApi = new RestApiOptions
                {
                    CommunicationServiceBaseAddress = TestBaseAddress,
                    CommunicationServiceLoginApiUrl = TestLoginApiUrl,
                    CommunicationServiceRequestApiUrl = TestRequestApiUrl,
                    ConfigurationServiceBaseAddress = TestBaseAddress,
                    ConfigurationServiceApiUrl = "/api/v1/Configuration",
                    ProcessingServiceBaseAddress = TestBaseAddress,
                    ProcessingServiceApiUrl = "/api/v1/Processing"
                },
                HttpClient = new HttpClientOptions
                {
                    HttpClientDefaultName = "test-client",
                    ShouldDisposeHttpClient = false,
                    RetryCountOnFailed = 0,
                    RetryCountOnAuth = 0
                }
            };
            var userServiceOptions = new UserServiceOptions
            {
                CanUseSystemUserToAuthenticate = false
            };

            return new CommunicationServiceClient(
                httpClientFactory, serviceOptions, userServiceOptions
            );
        }

        private static HttpResponseMessage CreateJsonResponse(HttpStatusCode statusCode, string body)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
        }

    }
}
