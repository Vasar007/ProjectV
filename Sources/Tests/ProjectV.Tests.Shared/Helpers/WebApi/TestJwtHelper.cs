using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Acolyte.Assertions;
using Microsoft.IdentityModel.Tokens;

namespace ProjectV.Tests.Shared.Helpers.WebApi
{
    /// <summary>
    /// Test helper that mints bearer tokens accepted by the real
    /// <c>AddJtwAuthentication</c> middleware. The helper signs each token
    /// with the same base64-encoded HMAC SHA-256 key, issuer, and audience
    /// that <see cref="TestWebApplicationFactory{TStartup}" /> seeds into the
    /// hosted-service configuration — so the production
    /// <c>TokenValidationParameters</c> accept the token end-to-end without
    /// any test-only bypass.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The shape mirrors
    /// <c>Sources/WebServices/ProjectV.CommonWebApi/Authorization/Tokens/Generators/TokenGenerator.cs</c>
    /// — same claim layout (<see cref="ClaimTypes.NameIdentifier" />), same
    /// <see cref="SecurityAlgorithms.HmacSha256Signature" /> alias, same
    /// <c>iss</c>/<c>aud</c> values.
    /// </para>
    /// <para>
    /// All defaulted parameters point at <see cref="TestJwtConfig" /> so any
    /// test can change a single field (e.g. <c>userId</c>) without rebuilding
    /// the whole bundle.
    /// </para>
    /// </remarks>
    public static class TestJwtHelper
    {
        /// <summary>
        /// Generates a signed bearer token suitable for use as the value of
        /// the HTTP <c>Authorization</c> header
        /// (<c>"Bearer "</c> + <see cref="GenerateTestBearerToken" />).
        /// </summary>
        /// <param name="secretKey">
        /// Base64-encoded HMAC SHA-256 key. Must match the production
        /// <c>JwtOptions.SecretKey</c> value seeded into the host's
        /// configuration by the test factory.
        /// </param>
        /// <param name="issuer">
        /// <c>iss</c> claim value. Must match the production
        /// <c>JwtOptions.Issuer</c>.
        /// </param>
        /// <param name="audience">
        /// <c>aud</c> claim value. Must match the production
        /// <c>JwtOptions.Audience</c>.
        /// </param>
        /// <param name="userId">
        /// Optional value to populate the <see cref="ClaimTypes.NameIdentifier" />
        /// claim. Mirrors how the production <c>TokenGenerator</c> stamps the
        /// user id into the access token.
        /// </param>
        /// <param name="userName">
        /// Optional value to populate the <see cref="ClaimTypes.Name" /> claim.
        /// </param>
        /// <param name="expiry">
        /// Optional lifetime offset; defaults to five minutes. Tokens with
        /// non-positive expiry let callers exercise the <c>ValidateLifetime</c>
        /// rejection path.
        /// </param>
        /// <returns>The serialised JWT bearer token.</returns>
        public static string GenerateTestBearerToken(
            string secretKey,
            string issuer,
            string audience,
            string? userId = null,
            string? userName = null,
            TimeSpan? expiry = null)
        {
            secretKey.ThrowIfNullOrWhiteSpace(nameof(secretKey));
            issuer.ThrowIfNullOrWhiteSpace(nameof(issuer));
            audience.ThrowIfNullOrWhiteSpace(nameof(audience));

            var key = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>();
            if (!string.IsNullOrEmpty(userId))
            {
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
            }
            if (!string.IsNullOrEmpty(userName))
            {
                claims.Add(new Claim(ClaimTypes.Name, userName));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-1),
                expires: DateTime.UtcNow.Add(expiry ?? TimeSpan.FromMinutes(5)),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Convenience overload that signs a token with the values bundled
        /// in a <see cref="TestJwtConfig" /> instance.
        /// </summary>
        /// <param name="config">Bundle of test-side signing material.</param>
        /// <param name="userId">Optional value for the user-id claim.</param>
        /// <param name="userName">Optional value for the user-name claim.</param>
        /// <param name="expiry">Optional token lifetime; default five minutes.</param>
        /// <returns>The serialised JWT bearer token.</returns>
        public static string GenerateTestBearerToken(
            TestJwtConfig config,
            string? userId = null,
            string? userName = null,
            TimeSpan? expiry = null)
        {
            config.ThrowIfNull(nameof(config));

            return GenerateTestBearerToken(
                secretKey: config.SecretKey,
                issuer: config.Issuer,
                audience: config.Audience,
                userId: userId,
                userName: userName,
                expiry: expiry
            );
        }
    }
}
