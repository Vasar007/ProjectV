using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AwesomeAssertions;
using ProjectV.Models.Data;
using ProjectV.Tests.Shared.Helpers.Fixtures;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ProjectV.OmdbService.Tests
{
    /// <summary>
    /// Contract-stage tests for <see cref="OmdbClient" />.
    /// Drives the real OMDbApiNet HTTP pipeline against an in-process
    /// <see cref="WireMockServer" /> that serves recorded JSON fixtures from
    /// <c>Sources/Tests/Fixtures/Omdb/</c>. No live API calls per Decision
    /// D-17; per-adapter failure isolation per Decision D-19.
    /// </summary>
    /// <remarks>
    /// <para>
    /// OMDbApiNet 1.3.0's <c>AsyncOmdbClient</c> hard-codes <c>BaseUrl =
    /// "http://www.omdbapi.com/?"</c> as a <c>const</c> field (verified via
    /// reflection during 02-08 research — reflection cannot patch <c>const</c>
    /// fields because the value is inlined at compile time). The SDK also
    /// instantiates a fresh <see cref="HttpClient" /> per call, so there is no
    /// per-instance handler seam to inject either. The remaining viable
    /// redirection seam is <see cref="HttpClient.DefaultProxy" />: setting it
    /// to a <see cref="WebProxy" /> pointing at the WireMock server routes
    /// every outbound HTTP request (including OMDb's hardcoded
    /// <c>http://www.omdbapi.com/</c> calls) through WireMock as a forward
    /// proxy. WireMock receives the original absolute URL and matches stubs
    /// against the request path / host accordingly.
    /// </para>
    /// <para>
    /// Setting <see cref="HttpClient.DefaultProxy" /> is process-global; this
    /// suite saves and restores the prior value across the
    /// <see cref="IAsyncLifetime" /> lifecycle so it does not bleed into
    /// other test classes in the same assembly. xUnit serialises tests
    /// within the same class by default, so the global proxy is owned
    /// exclusively by this class for the duration of its run.
    /// </para>
    /// </remarks>
    [Trait("Category", "Contract")]
    public sealed class OmdbContractTests : IAsyncLifetime
    {
        private const string MovieByTitleSuccessFixturePath = "Omdb/movie-by-title-success.json";
        private const string MovieByTitleNotFoundFixturePath = "Omdb/movie-by-title-not-found.json";

        private readonly WireMockServer _server;
        private readonly OmdbClient _sut;
        private readonly IWebProxy? _originalDefaultProxy;

        public OmdbContractTests()
        {
            _server = WireMockServer.Start();
            _originalDefaultProxy = HttpClient.DefaultProxy;

            // The api-key value is irrelevant — WireMock matches by path only
            // and the SDK echoes the key into the query string, not into auth
            // headers.
            _sut = new OmdbClient("test-key");
        }

        public Task InitializeAsync()
        {
            // Load fixtures + configure stubs FIRST, mutate the process-global
            // HttpClient.DefaultProxy LAST. xUnit v2 does NOT call
            // DisposeAsync when InitializeAsync throws; if the fixture file
            // were missing, mutating the proxy before the load would leak the
            // mutation for the rest of the test-runner's lifetime (and leak
            // the WireMock port). Doing the throwing work first means any
            // failure happens before the global is touched.
            //
            // Pitfall 3: raw-string body (NOT WithBodyAsJson + JObject.Parse)
            // — avoids WireMock.Net serializer / Newtonsoft.Json casing
            // conflict.
            string successBody = FixtureLoader.LoadJsonFixture(MovieByTitleSuccessFixturePath);

            // OMDb requests land at WireMock with the original absolute URL
            // (host = www.omdbapi.com, path = "/"). Stub by path "/" — that is
            // what the proxy-forwarded request resolves to.
            _server
                .Given(Request.Create().WithPath("/").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(successBody));

            // WireMockServer.Url is non-null after Start() returns; declared
            // string? for the lifecycle-pre-start state. This mutation is the
            // last operation in InitializeAsync so a failure earlier in this
            // method (e.g. fixture load) cannot leave the global state in a
            // half-applied state where the save/restore pair would not run.
            string wireMockUrl = _server.Url!;
            HttpClient.DefaultProxy = new WebProxy(new Uri(wireMockUrl));

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _sut.Dispose();
            HttpClient.DefaultProxy = _originalDefaultProxy!;
            _server.Stop();
            _server.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Verifies that <see cref="OmdbClient.TryGetItemByTitleAsync" /> drives
        /// a real HTTP GET through the forward-proxy seam, deserialises the
        /// recorded fixture, and surfaces the OMDb item shape through the
        /// mapped <see cref="OmdbMovieInfo" /> (<c>tt0012345</c> → numeric
        /// thing id <c>12345</c>; populated title; non-zero vote count).
        /// </summary>
        [Fact]
        public async Task TryGetItemByTitleAsyncReturnsExpectedMovie()
        {
            // Arrange.
            const string title = "Synthetic Movie";
            const int expectedThingId = 12345; // "tt0012345" → 12345 via mapper.

            // Act.
            OmdbMovieInfo? actualValue = await _sut.TryGetItemByTitleAsync(title);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.ThingId.Should().Be(expectedThingId);
            actualValue.Title.Should().Be("Synthetic Movie");
            actualValue.VoteCount.Should().Be(9876);
            _server.LogEntries.Should().HaveCount(1,
                "OmdbClient should make exactly one HTTP request for a successful by-title fetch " +
                "(no internal retry on a 200 response)");
        }

        /// <summary>
        /// Verifies that an OMDb <c>Response: "False"</c> envelope (the API's
        /// not-found shape, returned over HTTP 200) is short-circuited by the
        /// production wrapper into a <c>null</c> return — preserving the
        /// existing OmdbClient contract for the calling pipeline stages.
        /// </summary>
        [Fact]
        public async Task TryGetItemByTitleAsyncNotFoundReturnsNull()
        {
            // Arrange — override the success stub with the not-found envelope.
            // OMDb's not-found is a HTTP 200 with Response: "False".
            _server.Reset();
            string notFoundBody = FixtureLoader.LoadJsonFixture(MovieByTitleNotFoundFixturePath);
            _server
                .Given(Request.Create().WithPath("/").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(notFoundBody));

            // Act.
            OmdbMovieInfo? actualValue = await _sut.TryGetItemByTitleAsync("no-such-title");

            // Assert.
            actualValue.Should().BeNull(
                "OmdbClient should short-circuit Response:\"False\" envelopes into null");
            _server.LogEntries.Should().HaveCount(1,
                "OmdbClient should make exactly one HTTP request for the not-found path");
        }
    }
}
