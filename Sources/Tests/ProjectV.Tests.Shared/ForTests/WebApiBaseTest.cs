using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Tests.Shared.Helpers.WebApi;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for WebApi scenario tests that need an in-process host with
    /// JWT authentication wired the same way production wires it. Owns the
    /// <see cref="TestWebApplicationFactory{TStartup}" /> instance, exposes a
    /// default <see cref="HttpClient" /> for anonymous calls, and provides
    /// <see cref="CreateAuthenticatedClient" /> for calls that need a bearer
    /// token.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The factory is built lazily inside <see cref="InitializeAsync" /> so
    /// per-scenario base classes can hand a configured
    /// <see cref="TestJwtConfig" />, extra in-memory configuration overrides,
    /// or a DI override action to the base ctor and have the host pick them
    /// up. xUnit calls <see cref="InitializeAsync" /> before the first test
    /// runs and <see cref="DisposeAsync" /> after the last one — that is the
    /// public <see cref="IAsyncLifetime" /> contract.
    /// </para>
    /// <para>
    /// This base class deliberately does NOT depend on <c>DbCollectionFixture</c>;
    /// the JWT path in <c>CommunicationWebService</c> uses an in-memory user
    /// store. The Telegram webhook / polling families that DO need
    /// Testcontainers Postgres add the <c>[Collection(DbCollection.Name)]</c>
    /// attribute on their concrete subclass and pass the fixture through a
    /// derived base class — they do not extend this one.
    /// </para>
    /// </remarks>
    /// <typeparam name="TStartup">
    /// The production <c>Startup</c> class type that the test host wraps.
    /// </typeparam>
    public abstract class WebApiBaseTest<TStartup> : BaseMockTest, IAsyncLifetime
        where TStartup : class
    {
        private readonly IReadOnlyDictionary<string, string?> _extraConfiguration;
        private readonly Action<IServiceCollection> _configureTestServices;

        private TestWebApplicationFactory<TStartup>? _factory;
        private HttpClient? _client;

        /// <summary>
        /// Gets the lazily-initialised <see cref="TestWebApplicationFactory{TStartup}" />.
        /// Available after <see cref="InitializeAsync" /> has run.
        /// </summary>
        protected TestWebApplicationFactory<TStartup> Factory =>
            _factory ?? throw new InvalidOperationException(
                "TestWebApplicationFactory is not initialised yet — InitializeAsync must run first."
            );

        /// <summary>
        /// Gets a shared <see cref="HttpClient" /> built from <see cref="Factory" />
        /// without any <c>Authorization</c> header. Use it for anonymous-call
        /// scenarios.
        /// </summary>
        protected HttpClient Client =>
            _client ?? throw new InvalidOperationException(
                "HttpClient is not initialised yet — InitializeAsync must run first."
            );

        /// <summary>
        /// Gets the JWT signing material the factory was built with — exposed
        /// so a derived test can mint a token by hand if it needs claim
        /// customisation beyond <see cref="CreateAuthenticatedClient" />.
        /// </summary>
        protected TestJwtConfig JwtConfig { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="WebApiBaseTest{TStartup}" />.
        /// </summary>
        /// <param name="jwtConfig">
        /// Optional JWT signing-material bundle. Defaults to
        /// <see cref="TestJwtConfig" /> defaults (test-only base64 secret,
        /// <c>https://localhost</c> issuer + audience).
        /// </param>
        /// <param name="extraConfiguration">
        /// Optional in-memory configuration overrides layered on top of the
        /// host's <c>appsettings.json</c> / env vars. Useful for tweaking
        /// <c>UserServiceOptions</c> or other Options on a per-scenario basis.
        /// </param>
        /// <param name="configureTestServices">
        /// Optional DI override action; runs AFTER
        /// <c>Startup.ConfigureServices</c>. Defaults to a no-op.
        /// </param>
        protected WebApiBaseTest(
            TestJwtConfig? jwtConfig = null,
            IReadOnlyDictionary<string, string?>? extraConfiguration = null,
            Action<IServiceCollection>? configureTestServices = null)
        {
            JwtConfig = jwtConfig ?? new TestJwtConfig();
            _extraConfiguration = extraConfiguration ?? new Dictionary<string, string?>();
            _configureTestServices = configureTestServices ?? (_ => { });
        }

        /// <summary>
        /// Builds the <see cref="TestWebApplicationFactory{TStartup}" /> and
        /// the shared anonymous <see cref="Client" />. Called by xUnit before
        /// the first test method.
        /// </summary>
        public virtual Task InitializeAsync()
        {
            _factory = new TestWebApplicationFactory<TStartup>
            {
                JwtConfig = JwtConfig,
                ExtraConfigurationValues = _extraConfiguration,
                ConfigureTestServices = _configureTestServices
            };

            _client = _factory.CreateClient();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Disposes the factory and the shared client. Called by xUnit after
        /// the last test method runs.
        /// </summary>
        public virtual async Task DisposeAsync()
        {
            _client?.Dispose();
            _client = null;

            if (_factory is not null)
            {
                await _factory.DisposeAsync();
                _factory = null;
            }
        }

        /// <summary>
        /// Builds a fresh <see cref="HttpClient" /> with an
        /// <c>Authorization: Bearer &lt;token&gt;</c> header. The token is
        /// signed with the same secret / issuer / audience the host was
        /// configured with, so the production
        /// <c>TokenValidationParameters</c> accept it.
        /// </summary>
        /// <param name="userId">
        /// Optional user-id claim value (mapped to
        /// <c>ClaimTypes.NameIdentifier</c>). Mirrors the production
        /// <c>TokenGenerator</c> claim layout.
        /// </param>
        /// <param name="userName">
        /// Optional user-name claim value (mapped to <c>ClaimTypes.Name</c>).
        /// </param>
        /// <param name="expiry">
        /// Optional token lifetime; defaults to five minutes.
        /// </param>
        /// <returns>
        /// A new <see cref="HttpClient" /> instance with the bearer token
        /// attached. The caller is responsible for disposing it.
        /// </returns>
        protected HttpClient CreateAuthenticatedClient(
            string? userId = null,
            string? userName = null,
            TimeSpan? expiry = null)
        {
            var token = TestJwtHelper.GenerateTestBearerToken(
                config: JwtConfig,
                userId: userId,
                userName: userName,
                expiry: expiry
            );

            var client = Factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return client;
        }
    }
}
