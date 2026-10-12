using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace ProjectV.CommunicationWebService.Tests.Scenarios.Jwt
{
    /// <summary>
    /// Scenario JWT-2: Authenticated request to <c>/api/v1/Requests</c> is
    /// accepted by the JWT pipeline.
    /// </summary>
    /// <remarks>
    /// A valid bearer token signed with the same secret / issuer / audience
    /// the host was configured with must pass the
    /// <c>TokenValidationParameters</c> check in
    /// <c>AddJtwAuthentication</c>. The scenario does NOT assert on the
    /// response body shape — the request body is intentionally empty so the
    /// underlying configuration receiver may reject it with a 400 — what
    /// matters is that the response is NOT 401 / 403 (i.e. the auth pipeline
    /// let the request through).
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class JwtAuthenticatedRequestTests : JwtAuthScenarioBaseTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="JwtAuthenticatedRequestTests" /> class.
        /// </summary>
        public JwtAuthenticatedRequestTests()
        {
        }

        /// <summary>
        /// Scenario JWT-2 — authenticated POST passes the JWT bearer
        /// pipeline (status is anything except 401 / 403).
        /// </summary>
        [Fact]
        public async Task RequestToProtectedEndpoint_WithValidToken_PassesAuthPipeline()
        {
            // Arrange.
            using HttpClient authenticatedClient = CreateAuthenticatedClient(
                userId: "00000000-0000-0000-0000-0000000000A1",
                userName: "integration-test-user");
            using var content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act.
            using HttpResponseMessage response = await authenticatedClient.PostAsync(
                "/api/v1/Requests", content);

            // Assert.
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized,
                "a valid bearer token must pass the JWT bearer middleware");
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden,
                "a valid bearer token without role claims must not be forbidden by the default policy");
        }
    }
}
