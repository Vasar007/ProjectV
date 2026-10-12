using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AwesomeAssertions;
using Xunit;

namespace ProjectV.CommunicationWebService.Tests.Scenarios.Jwt
{
    /// <summary>
    /// Scenario JWT-1: Anonymous request to <c>/api/v1/Requests</c> is
    /// rejected.
    /// </summary>
    /// <remarks>
    /// When no <c>Authorization</c> header is attached to a request that
    /// targets <c>POST /api/v1/Requests</c> — a controller action decorated
    /// with <c>[Authorize]</c> in
    /// <c>ProjectV.CommunicationWebService.v1.Controllers.RequestsController</c>
    /// — the production JWT bearer middleware
    /// (<c>AddJtwAuthentication</c> in <c>ProjectV.CommonWebApi</c>) must
    /// short-circuit the pipeline with HTTP 401 Unauthorized.
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class JwtAnonymousRequestTests : JwtAuthScenarioBaseTest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="JwtAnonymousRequestTests" /> class.
        /// </summary>
        public JwtAnonymousRequestTests()
        {
        }

        /// <summary>
        /// Scenario JWT-1 — anonymous POST is rejected with HTTP 401.
        /// </summary>
        [Fact]
        public async Task RequestToProtectedEndpoint_WithoutToken_Returns401()
        {
            // Arrange.
            using var content = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act.
            using HttpResponseMessage response = await Client.PostAsync(
                "/api/v1/Requests", content);

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized,
                "unauthenticated requests must be rejected with 401");
        }
    }
}
