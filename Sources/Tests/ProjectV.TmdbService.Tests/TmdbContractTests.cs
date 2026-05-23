using System;
using System.Threading.Tasks;
using AwesomeAssertions;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Fixtures;
using ProjectV.TmdbService.Models;
using TMDbLib.Client;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace ProjectV.TmdbService.Tests
{
    /// <summary>
    /// Contract-stage tests for <see cref="TmdbClient" />.
    /// Drives the real TMDbLib HTTP pipeline against an in-process
    /// <see cref="WireMockServer" /> that serves recorded JSON fixtures from
    /// <c>Sources/Tests/Fixtures/Tmdb/</c>. No live API calls per Decision
    /// D-17; per-adapter failure isolation per Decision D-19.
    /// </summary>
    /// <remarks>
    /// The TMDbLib <see cref="TMDbClient" /> ctor accepts a <c>baseUrl</c>
    /// parameter (host:port, no scheme — TMDbLib prefixes <c>http://</c> when
    /// <c>useSsl: false</c>). Constructing the SDK client with the WireMock
    /// server's host:port lets the real TMDb HTTP plumbing run end-to-end
    /// (request building, query-string composition, JSON deserialization,
    /// internal retry policy) while the bytes on the wire are sourced from
    /// pinned in-repo fixtures.
    ///
    /// The production <see cref="ProjectV.TmdbService.TmdbClient" /> wrapper
    /// exposes <c>TrySearchMovieAsync</c> and <c>GetConfigAsync</c> (no
    /// <c>GetMovieAsync(int)</c> exists despite the plan wording — the SUT
    /// surface is verified against the actual public API).
    /// </remarks>
    [Trait("Category", "Contract")]
    public sealed class TmdbContractTests : BaseMockTest, IAsyncLifetime
    {
        private const string SearchMovieFixturePath = "Tmdb/search-movie-success.json";
        private const string SearchMovieEmptyFixturePath = "Tmdb/search-movie-empty.json";
        private const string ConfigurationFixturePath = "Tmdb/configuration-success.json";

        private readonly WireMockServer _server;
        private readonly TmdbClient _sut;

        public TmdbContractTests()
        {
            // Random localhost port; lifecycle owned by IAsyncLifetime hooks.
            _server = WireMockServer.Start();

            // useSsl: false so the SDK speaks plain HTTP to the local stub.
            // baseUrl: WireMock's host:port (TMDbLib prefixes http:// itself).
            // WireMockServer.Url is non-null after Start() returns; the
            // declared type is string? for the lifecycle-pre-start state.
            string wireMockUrl = _server.Url!;
            var uri = new Uri(wireMockUrl);
            string hostPort = $"{uri.Host}:{uri.Port}";
            _sut = new TmdbClient(
                apiKey: "test-key",
                useSsl: false,
                baseUrl: hostPort
            );
        }

        public Task InitializeAsync()
        {
            // Stub /3/search/movie GET → recorded success container.
            // Pitfall 3: raw-string body via FixtureLoader (NOT WithBodyAsJson +
            // JObject.Parse) — avoids WireMock.Net serializer / Newtonsoft.Json
            // casing conflict that mangles property names.
            string searchSuccess = FixtureLoader.LoadJsonFixture(SearchMovieFixturePath);
            _server
                .Given(Request.Create().WithPath("/3/search/movie").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(searchSuccess));

            // Stub /3/configuration GET → recorded configuration envelope.
            string configurationSuccess = FixtureLoader.LoadJsonFixture(ConfigurationFixturePath);
            _server
                .Given(Request.Create().WithPath("/3/configuration").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(configurationSuccess));

            return Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            _sut.Dispose();
            _server.Stop();
            _server.Dispose();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Verifies that <see cref="TmdbClient.TrySearchMovieAsync" /> drives a
        /// real HTTP GET against <c>/3/search/movie</c>, deserialises the
        /// recorded fixture, and returns a populated
        /// <see cref="TmdbSearchContainer" /> with the expected sentinel id.
        /// </summary>
        [Fact]
        public async Task TrySearchMovieAsyncReturnsExpectedContainer()
        {
            // Arrange.
            const string query = "synthetic";
            const int expectedThingId = 12345;

            // Act.
            TmdbSearchContainer? actualValue = await _sut.TrySearchMovieAsync(query);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Results.Should().HaveCount(1);
            actualValue.Results[0].ThingId.Should().Be(expectedThingId);
            actualValue.Results[0].Title.Should().NotBeNullOrWhiteSpace();
            _server.LogEntries.Should().HaveCount(1,
                "TmdbClient should make exactly one HTTP request for a successful search " +
                "(no internal retry on a 200 response)");
        }

        /// <summary>
        /// Verifies that a zero-results TMDb response (empty <c>results</c>
        /// array) is deserialised into an empty
        /// <see cref="TmdbSearchContainer" /> — the SDK does not return null
        /// for a well-formed empty envelope; the production wrapper preserves
        /// that behaviour.
        /// </summary>
        [Fact]
        public async Task TrySearchMovieAsyncEmptyResultReturnsEmptyContainer()
        {
            // Arrange — override the success stub with the empty-envelope fixture.
            _server.Reset();
            string searchEmpty = FixtureLoader.LoadJsonFixture(SearchMovieEmptyFixturePath);
            _server
                .Given(Request.Create().WithPath("/3/search/movie").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(searchEmpty));

            // Act.
            TmdbSearchContainer? actualValue = await _sut.TrySearchMovieAsync("no-such-movie");

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.Results.Should().BeEmpty();
            actualValue.TotalResults.Should().Be(0);
            _server.LogEntries.Should().HaveCount(1,
                "TmdbClient should make exactly one HTTP request for the empty-result path " +
                "(no internal retry on a well-formed 200)");
        }

        /// <summary>
        /// Verifies that <see cref="TmdbClient.GetConfigAsync" /> drives a
        /// real HTTP GET against <c>/3/configuration</c>, deserialises the
        /// recorded fixture, and surfaces the image base URL + poster sizes
        /// through the mapped
        /// <c>TmdbServiceConfigurationInfo</c>.
        /// </summary>
        [Fact]
        public async Task GetConfigAsyncReturnsExpectedConfig()
        {
            // Act.
            var actualValue = await _sut.GetConfigAsync();

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.BaseUrl.Should().Be("http://image.example.test/t/p/");
            actualValue.SecureBaseUrl.Should().Be("https://image.example.test/t/p/");
            actualValue.PosterSizes.Should().NotBeEmpty();
            actualValue.BackdropSizes.Should().NotBeEmpty();
            _server.LogEntries.Should().HaveCount(1,
                "TmdbClient should make exactly one HTTP request for the configuration fetch");
        }
    }
}
