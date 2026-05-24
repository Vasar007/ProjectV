using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ProjectV.Core.Services.Clients;
using Telegram.Bot;

namespace ProjectV.Tests.Shared.Helpers.WebApi
{
    /// <summary>
    /// Generic <see cref="WebApplicationFactory{TEntryPoint}" /> wrapper used
    /// by every ProjectV web-service integration suite. The factory wires up
    /// the host with deterministic test-side overrides:
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>
    ///     <description>
    ///     <see cref="JwtConfig" /> base64 secret + issuer + audience are
    ///     injected into the host's <see cref="IConfiguration" /> BEFORE the
    ///     <c>Startup.ConfigureServices</c> call that wires
    ///     <c>AddJtwAuthentication(jwtConfig)</c> — this is the only seam
    ///     that lets us swap the signing key without forking the host code,
    ///     because the JWT bearer middleware reads the secret at registration
    ///     time, not on each request.
    ///   </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     Per-test <see cref="ExtraConfigurationValues" /> may layer
    ///     additional in-memory configuration on top (for example a system
    ///     user name/password for the JWT login round-trip scenario, or a
    ///     dummy <c>BotToken</c> so the Telegram bot host can start without
    ///     a real Telegram API token).
    ///   </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <see cref="ConfigureTestServices" /> is the post-<c>Startup</c>
    ///     seam — it runs AFTER <c>Startup.ConfigureServices</c>, so DI
    ///     overrides (e.g. an empty <c>IUserInfoService</c> substitute for
    ///     scenarios that should NOT include a system user, or an
    ///     <c>IBotService</c> stub for Telegram tests) replace the
    ///     production registration. The default delegate is a no-op.
    ///   </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <see cref="TelegramBotClientStub" /> is exposed for downstream
    ///     scenario base classes that need to read the stub back (e.g. to
    ///     assert outgoing bot calls through <c>Received()</c>). The
    ///     factory itself does NOT register this stub into the DI
    ///     container — that lives in the per-family base class because
    ///     <c>IBotService</c> is defined in the Telegram bot host assembly
    ///     and we deliberately avoid taking that project reference here.
    ///     Defaults to <c>null</c>.
    ///   </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <see cref="CommunicationServiceClientStub" /> swaps the
    ///     production <see cref="ICommunicationServiceClient" /> singleton
    ///     so bot handlers that schedule downstream work do not contact the
    ///     real <c>CommunicationWebService</c>. Webhook tests leave this
    ///     <c>null</c> because the webhook path does not touch the comm-client;
    ///     polling tests supply one built via
    ///     <c>TestCommunicationServiceClientBuilder</c>.
    ///   </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     The host environment is forced to <see cref="Environments.Development" />
    ///     so HSTS / HTTPS-redirection branches in <c>Startup.Configure</c>
    ///     stay out of the way; the test client follows redirects by default.
    ///   </description>
    ///   </item>
    /// </list>
    /// <para>
    /// The <typeparamref name="TStartup" /> generic argument is the
    /// production <c>Startup</c> class (NOT <c>Program</c>) — ProjectV web
    /// services use the non-minimal <c>UseStartup&lt;Startup&gt;()</c> host
    /// builder (see <c>Sources/WebServices/ProjectV.CommunicationWebService/Program.cs</c>).
    /// </para>
    /// </remarks>
    /// <typeparam name="TStartup">
    /// The production <c>Startup</c> class type that the test host wraps.
    /// </typeparam>
    public class TestWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        /// <summary>
        /// Gets or sets the JWT signing-material bundle that the factory
        /// pushes into the host's <see cref="IConfiguration" /> so the
        /// production <c>AddJtwAuthentication</c> registration accepts
        /// tokens signed by <see cref="TestJwtHelper" />.
        /// </summary>
        public TestJwtConfig JwtConfig { get; init; } = new TestJwtConfig();

        /// <summary>
        /// Gets or sets extra key-value pairs layered on top of the host's
        /// configuration via
        /// <see cref="IConfigurationBuilder.AddInMemoryCollection(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{string, string}})" />.
        /// Use this to override individual options (e.g. <c>UserServiceOptions</c>
        /// for the login-round-trip scenario).
        /// </summary>
        public IReadOnlyDictionary<string, string?> ExtraConfigurationValues { get; init; } =
            new Dictionary<string, string?>();

        /// <summary>
        /// Gets or sets a hook that lets a per-scenario base class swap or
        /// remove DI registrations on top of <c>Startup.ConfigureServices</c>.
        /// Defaults to a no-op.
        /// </summary>
        public Action<IServiceCollection> ConfigureTestServices { get; init; } =
            _ => { };

        /// <summary>
        /// Gets or sets an optional <see cref="ITelegramBotClient" />
        /// substitute (typically built via <c>TestTelegramBotClientBuilder</c>).
        /// The factory itself does not register this stub into DI — that
        /// is the responsibility of the per-family base class that knows
        /// about <c>IBotService</c> (which lives in the Telegram bot
        /// host assembly and is intentionally NOT referenced from
        /// <c>ProjectV.Tests.Shared</c>). Defaults to <c>null</c>.
        /// </summary>
        public ITelegramBotClient? TelegramBotClientStub { get; init; }

        /// <summary>
        /// Gets or sets an optional <see cref="ICommunicationServiceClient" />
        /// substitute (typically built via
        /// <c>TestCommunicationServiceClientBuilder</c>) that replaces the
        /// production registration inside the test host. When
        /// non-<c>null</c>, the production <see cref="ICommunicationServiceClient" />
        /// transient is removed and re-registered with this stub instance.
        /// Defaults to <c>null</c> (production wiring stands — useful when
        /// the host does not reach the comm-client on the path under test,
        /// e.g. the Telegram webhook path).
        /// </summary>
        public ICommunicationServiceClient? CommunicationServiceClientStub { get; init; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestWebApplicationFactory{TStartup}" />.
        /// </summary>
        public TestWebApplicationFactory()
        {
        }

        /// <inheritdoc />
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment(Environments.Development);

            builder.ConfigureAppConfiguration((_, configBuilder) =>
            {
                // The JWT bearer middleware reads the secret/issuer/audience
                // INSIDE Startup.ConfigureServices via
                // AddJtwAuthentication(jwtConfig). PostConfigure<JwtOptions>
                // runs too late — we need to set the values in
                // IConfiguration BEFORE Startup sees them. AddInMemoryCollection
                // is layered on top of the production appsettings.json / env
                // vars, so it wins.
                var overrides = new Dictionary<string, string?>
                {
                    [$"JwtOptions:SecretKey"] = JwtConfig.SecretKey,
                    [$"JwtOptions:Issuer"] = JwtConfig.Issuer,
                    [$"JwtOptions:Audience"] = JwtConfig.Audience,
                };

                foreach (var pair in ExtraConfigurationValues)
                {
                    overrides[pair.Key] = pair.Value;
                }

                configBuilder.AddInMemoryCollection(overrides);
            });

            builder.ConfigureTestServices(services =>
            {
                if (CommunicationServiceClientStub is not null)
                {
                    services.RemoveAll<ICommunicationServiceClient>();
                    services.AddSingleton(CommunicationServiceClientStub);
                }

                ConfigureTestServices(services);
            });
        }
    }
}
