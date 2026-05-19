using System;

namespace ProjectV.Tests.Shared.Helpers.WebApi
{
    /// <summary>
    /// Bundle of test-side JWT configuration values that
    /// <see cref="TestWebApplicationFactory{TStartup}" /> seeds into the
    /// hosted-service configuration so the JWT bearer middleware (which is
    /// wired at <c>ConfigureServices</c> time inside
    /// <c>AddJtwAuthentication(jwtConfig)</c>) signs and validates tokens
    /// with the SAME secret / issuer / audience that
    /// <see cref="TestJwtHelper.GenerateTestBearerToken" /> uses on the
    /// test side.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The secret is a base64-encoded value because that is the shape the
    /// production <c>JwtOptions.SecretKey</c> contract uses (see
    /// <c>Sources/WebServices/ProjectV.CommonWebApi/Authorization/Tokens/Generators/TokenGenerator.cs</c>
    /// — it calls <see cref="Convert.FromBase64String" />). The default
    /// value in this class is a constant test-only key; production secrets
    /// never enter the test code.
    /// </para>
    /// <para>
    /// The default <see cref="Issuer" /> and <see cref="Audience" /> match
    /// the <c>appsettings.json</c> baseline for
    /// <c>ProjectV.CommunicationWebService</c> so the same factory works
    /// against the in-tree configuration if it is partially merged.
    /// </para>
    /// </remarks>
    public sealed class TestJwtConfig
    {
        /// <summary>
        /// Default base64-encoded HMAC SHA-256 key used by integration tests.
        /// This is a literal test value — never reused outside the test suite.
        /// </summary>
        public const string DefaultSecretKeyBase64 =
            "VGVzdC1Pbmx5LUp3dC1TZWNyZXQtRm9yLVByb2plY3RWLUludGVnci10ZXN0cy0wMQ==";

        /// <summary>
        /// Default token issuer; aligned with the production
        /// <c>appsettings.json</c> baseline.
        /// </summary>
        public const string DefaultIssuer = "https://localhost";

        /// <summary>
        /// Default token audience; aligned with the production
        /// <c>appsettings.json</c> baseline.
        /// </summary>
        public const string DefaultAudience = "https://localhost";

        /// <summary>
        /// Gets the base64-encoded signing key.
        /// </summary>
        public string SecretKey { get; init; } = DefaultSecretKeyBase64;

        /// <summary>
        /// Gets the JWT <c>iss</c> claim value.
        /// </summary>
        public string Issuer { get; init; } = DefaultIssuer;

        /// <summary>
        /// Gets the JWT <c>aud</c> claim value.
        /// </summary>
        public string Audience { get; init; } = DefaultAudience;

        /// <summary>
        /// Initializes a new instance of <see cref="TestJwtConfig" />.
        /// </summary>
        public TestJwtConfig()
        {
        }
    }
}
