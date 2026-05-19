using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    ///     user name/password for the JWT login round-trip scenario).
    ///   </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///     <see cref="ConfigureTestServices" /> is the post-<c>Startup</c>
    ///     seam — it runs AFTER <c>Startup.ConfigureServices</c>, so DI
    ///     overrides (e.g. an empty <c>IUserInfoService</c> substitute for
    ///     scenarios that should NOT include a system user) replace the
    ///     production registration. The default delegate is a no-op.
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
                ConfigureTestServices(services);
            });
        }
    }
}
