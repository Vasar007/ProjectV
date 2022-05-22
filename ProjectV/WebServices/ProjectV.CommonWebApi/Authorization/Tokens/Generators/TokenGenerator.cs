using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectV.CommonWebApi.Models.Options;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Generators
{
    public sealed class TokenGenerator : ITokenGenerator
    {
        private readonly JwtOptions _settings;

        private TimeSpan AccessTokenExpirationTimeout => _settings.AccessTokenExpirationTimeout;
        private TimeSpan RefreshTokenExpirationTimeout => _settings.RefreshTokenExpirationTimeout;


        public TokenGenerator(
            IOptions<JwtOptions> settingsOptions)
        {
            _settings = settingsOptions.Value.ThrowIfNull(nameof(settingsOptions));
        }

        #region ITokenGenerator Implementation

        public async Task<AccessTokenData> GenerateAccessTokenAsync(UserId userId,
            DateTime utcDateTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_settings.SecretKey);

            var claimsIdentity = new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }
            );

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            );

            var expires = utcDateTime.Add(AccessTokenExpirationTimeout);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                Expires = expires,
                SigningCredentials = signingCredentials,
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = await Task.Run(() => tokenHandler.WriteToken(securityToken));

            return new AccessTokenData(accessToken, expires);
        }

        public async Task<RefreshTokenData> GenerateRefreshTokenAsync(DateTime utcDateTime)
        {
            var secureRandomBytes = new byte[32];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            await Task.Run(() => randomNumberGenerator.GetBytes(secureRandomBytes));

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            var expires = utcDateTime.Add(RefreshTokenExpirationTimeout);

            return new RefreshTokenData(refreshToken, utcDateTime);
        }

        #endregion
    }
}
