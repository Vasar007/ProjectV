using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectV.CommonWebApi.Models.Options;
using ProjectV.Configuration;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Generators
{
    public sealed class TokenGenerator : ITokenGenerator
    {
        private readonly JwtOptions _options;

        private TimeSpan AccessTokenExpirationTimeout => _options.AccessTokenExpirationTimeout;
        private TimeSpan RefreshTokenExpirationTimeout => _options.RefreshTokenExpirationTimeout;


        public TokenGenerator(
            IOptions<JwtOptions> options)
        {
            _options = options.GetCheckedValue();
        }

        #region ITokenGenerator Implementation

        public async Task<AccessTokenData> GenerateAccessTokenAsync(UserId userId,
            DateTime utcDateTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_options.SecretKey);

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
                Issuer = _options.Issuer,
                Audience = _options.Audience,
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
