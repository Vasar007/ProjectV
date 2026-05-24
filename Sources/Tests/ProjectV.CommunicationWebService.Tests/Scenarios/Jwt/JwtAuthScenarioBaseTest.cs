using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.WebApi;

namespace ProjectV.CommunicationWebService.Tests.Scenarios.Jwt
{
    /// <summary>
    /// Per-family base class for JWT scenario tests against
    /// <c>ProjectV.CommunicationWebService</c>. Bundles the
    /// <see cref="TestWebApplicationFactory{TStartup}" /> wiring + any
    /// JWT-specific configuration overrides that every scenario in the
    /// family inherits.
    /// </summary>
    /// <remarks>
    /// The default <see cref="TestJwtConfig" /> is shared across the suite,
    /// so a single base64 secret signs tokens for every test.
    /// <c>UserServiceOptions</c> is left in its <c>appsettings.json</c>
    /// baseline (system user is created on startup) — individual scenarios
    /// can layer additional in-memory configuration on top by handing
    /// extra key-value pairs through the protected constructor.
    /// </remarks>
    public abstract class JwtAuthScenarioBaseTest : WebApiBaseTest<Startup>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="JwtAuthScenarioBaseTest" /> class with default JWT
        /// signing material and no extra configuration overrides.
        /// </summary>
        protected JwtAuthScenarioBaseTest()
            : this(extraConfiguration: null, configureTestServices: null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="JwtAuthScenarioBaseTest" /> class.
        /// </summary>
        /// <param name="extraConfiguration">
        /// Optional in-memory configuration overrides layered on top of the
        /// host's <c>appsettings.json</c> — for example a custom
        /// <c>UserServiceOptions:SystemUserName</c> / <c>:SystemUserPassword</c>
        /// pair for the login round-trip scenario.
        /// </param>
        /// <param name="configureTestServices">
        /// Optional DI override action that runs AFTER
        /// <c>Startup.ConfigureServices</c>.
        /// </param>
        protected JwtAuthScenarioBaseTest(
            IReadOnlyDictionary<string, string?>? extraConfiguration,
            Action<IServiceCollection>? configureTestServices)
            : base(
                jwtConfig: null,
                extraConfiguration: extraConfiguration,
                configureTestServices: configureTestServices)
        {
        }
    }
}
