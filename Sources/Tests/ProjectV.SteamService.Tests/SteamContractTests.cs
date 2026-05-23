using System.Reflection;
using System.Threading.Tasks;
using AwesomeAssertions;
using ProjectV.Models.Data;
using ProjectV.SteamService.Models;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Fixtures;
using SteamWebApiLib;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

// Disambiguate: ProjectV's SteamApiClient is the SUT; SteamWebApiLib's is the
// internal third-party SDK. Both live in their respective namespaces; we alias
// to keep call sites readable.
using ProjectVSteamApiClient = ProjectV.SteamService.SteamApiClient;
using SdkSteamApiClient = SteamWebApiLib.SteamApiClient;

namespace ProjectV.SteamService.Tests
{
    /// <summary>
    /// Contract-stage tests for <see cref="ProjectVSteamApiClient" />.
    /// Drives the real SteamWebApiLib HTTP pipeline against an in-process
    /// <see cref="WireMockServer" /> that serves recorded JSON fixtures from
    /// <c>Sources/Tests/Fixtures/Steam/</c>. No live API calls per Decision
    /// D-17; per-adapter failure isolation per Decision D-19.
    /// </summary>
    /// <remarks>
    /// <see cref="SteamApiConfig" /> exposes writable
    /// <c>SteamPoweredBaseUrl</c> and <c>SteamStoreBaseUrl</c> properties —
    /// pointing both at the WireMock server's URL routes the GetAppList
    /// (api.steampowered.com → <c>/ISteamApps/GetAppList/v0002/</c>) and the
    /// appdetails (store.steampowered.com → <c>/api/appdetails</c>)
    /// endpoints to the local stub. The production wrapper's single-arg ctor
    /// builds its own <see cref="SteamApiConfig" /> internally, so we use
    /// reflection on the private <c>_steamApiClient</c> field to replace the
    /// SDK instance with one built from a config whose base URLs point at
    /// WireMock. <c>RetryAttempts</c> is set to 0 so the HTTP log-entry count
    /// assertion can rely on exactly-once semantics on the success path.
    /// </remarks>
    [Trait("Category", "Contract")]
    public sealed class SteamContractTests : BaseMockTest, IAsyncLifetime
    {
        private const int ExpectedAppId = 730;
        private const string AppListFixturePath = "Steam/app-list-success.json";
        private const string AppDetailFixturePath = "Steam/app-detail-success.json";

        private readonly WireMockServer _server;
        private readonly ProjectVSteamApiClient _sut;

        public SteamContractTests()
        {
            _server = WireMockServer.Start();

            // Build a config whose base URLs point at WireMock. RetryAttempts=0
            // keeps the HTTP-call counts deterministic for the exactly-once
            // log-entry assertion.
            var overriddenConfig = new SteamApiConfig
            {
                ApiKey = "test-key",
                SteamPoweredBaseUrl = _server.Url,
                SteamStoreBaseUrl = _server.Url,
                RetryAttempts = 0
            };

            // ProjectVSteamApiClient ctor builds its own SteamApiConfig from
            // the api-key alone; replace the internal SDK instance with one
            // built from our overridden config (InternalsVisibleTo grants
            // access to the internal sealed type).
            _sut = new ProjectVSteamApiClient("test-key");
            ReplaceInternalSdkClient(overriddenConfig);
        }

        public Task InitializeAsync()
        {
            // Stub /ISteamApps/GetAppList/v0002/ GET → recorded app list.
            // Pitfall 3: raw-string body (NOT WithBodyAsJson + JObject.Parse)
            // — avoids WireMock.Net serializer / Newtonsoft.Json casing
            // conflict.
            string appList = FixtureLoader.LoadJsonFixture(AppListFixturePath);
            _server
                .Given(Request.Create().WithPath("/ISteamApps/GetAppList/v0002/").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(appList));

            // Stub /api/appdetails GET → recorded app-detail envelope.
            string appDetail = FixtureLoader.LoadJsonFixture(AppDetailFixturePath);
            _server
                .Given(Request.Create().WithPath("/api/appdetails").UsingGet())
                .RespondWith(Response.Create()
                    .WithStatusCode(200)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithBody(appDetail));

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
        /// Verifies that <see cref="ProjectVSteamApiClient.GetAppListAsync" />
        /// drives a real HTTP GET against
        /// <c>/ISteamApps/GetAppList/v0002/</c>, deserialises the recorded
        /// fixture, and surfaces the brief-info container through the mapped
        /// <see cref="SteamBriefInfoContainer" />. The fixture pins 3 entries
        /// and a sentinel app-id <c>730</c> for the first row.
        /// </summary>
        [Fact]
        public async Task GetAppListAsyncReturnsExpectedApps()
        {
            // Act.
            SteamBriefInfoContainer actualValue = await _sut.GetAppListAsync();

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Results.Should().HaveCount(3);
            actualValue.Results[0].AppId.Should().Be(ExpectedAppId);
            actualValue.Results[0].Name.Should().NotBeNullOrWhiteSpace();
            _server.LogEntries.Should().HaveCount(1,
                "SteamApiClient should make exactly one HTTP request for a successful app-list fetch " +
                "(no internal retry on a 200 response, RetryAttempts=0)");
        }

        /// <summary>
        /// Verifies that
        /// <see cref="ProjectVSteamApiClient.TryGetSteamAppAsync" /> drives a
        /// real HTTP GET against <c>/api/appdetails</c>, deserialises the
        /// recorded fixture, and surfaces the app envelope (success: true,
        /// data: SteamApp) through the mapped <see cref="SteamGameInfo" />.
        /// The fixture pins <c>steam_appid=730</c>; the mapped
        /// <c>ThingId</c> matches.
        /// </summary>
        [Fact]
        public async Task TryGetSteamAppAsyncReturnsExpectedApp()
        {
            // Arrange.
            const SteamCountryCode countryCode = SteamCountryCode.USA;
            const SteamResponseLanguage language = SteamResponseLanguage.English;

            // Act.
            SteamGameInfo? actualValue = await _sut.TryGetSteamAppAsync(
                ExpectedAppId, countryCode, language);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue!.ThingId.Should().Be(ExpectedAppId);
            actualValue.Title.Should().NotBeNullOrWhiteSpace();
            actualValue.RequiredAge.Should().BeGreaterThanOrEqualTo(0);
            actualValue.PosterPath.Should().NotBeNullOrWhiteSpace();
            _server.LogEntries.Should().HaveCount(1,
                "SteamApiClient should make exactly one HTTP request for a successful by-id fetch " +
                "(no internal retry on a 200 response, RetryAttempts=0)");
        }

        /// <summary>
        /// Replaces the third-party
        /// <see cref="SdkSteamApiClient" /> instance held by the production
        /// wrapper with one built from a <see cref="SteamApiConfig" /> whose
        /// base URLs point at WireMock. The production
        /// <see cref="ProjectVSteamApiClient" /> ctor accepts only an
        /// api-key and builds its own config internally, so the overridden
        /// base URLs cannot reach the SDK without reflection on the private
        /// SDK instance field.
        /// </summary>
        private void ReplaceInternalSdkClient(SteamApiConfig overriddenConfig)
        {
            // FRAGILE: private-field reflection seam. If SteamWebApiLib renames
            // _steamApiClient, converts it to a property, or changes the
            // SdkSteamApiClient ctor surface, the assertion below fires at
            // runtime (not compile time) and this contract suite breaks. The
            // documented Rule-3 deviation in 02-08-SUMMARY.md accepts this
            // fragility because ProjectVSteamApiClient's single-arg ctor builds
            // its own SdkSteamApiClient internally — there is no public seam to
            // inject a SteamApiConfig pointing at WireMock. The non-fragile fix
            // is a ctor overload on ProjectVSteamApiClient that accepts a
            // pre-built SteamApiConfig; until then, watch for SDK upgrade
            // breakage on this line.
            FieldInfo? sdkFieldInfo = typeof(ProjectVSteamApiClient).GetField(
                "_steamApiClient",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            sdkFieldInfo.Should().NotBeNull(
                "ProjectV.SteamService.SteamApiClient must hold the SDK client in _steamApiClient " +
                "for the contract-test seam to redirect outbound calls to WireMock");

            // Dispose the SDK instance built by the production ctor (which
            // pointed at the live Steam endpoints) before replacing it so we
            // do not leak the HttpClient it owns.
            (sdkFieldInfo!.GetValue(_sut) as SdkSteamApiClient)?.Dispose();
            sdkFieldInfo.SetValue(_sut, new SdkSteamApiClient(overriddenConfig));
        }
    }
}
