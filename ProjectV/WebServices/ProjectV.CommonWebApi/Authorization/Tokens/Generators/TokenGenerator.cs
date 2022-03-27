using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ProjectV.CommonWebApi.Models.Config;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Generators
{
    public sealed class TokenGenerator : ITokenGenerator
    {
        private readonly JwtConfiguration _settings;


        public TokenGenerator(
            IOptions<JwtConfiguration> settingsOptions)
        {
            _settings = settingsOptions.Value.ThrowIfNull(nameof(settingsOptions));
        }

        #region ITokenGenerator Implementation

        public async Task<string> GenerateAccessTokenAsync(UserId userId)
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

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                Expires = DateTime.Now.AddHours(3),
                SigningCredentials = signingCredentials,
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);

            return await Task.Run(() => tokenHandler.WriteToken(securityToken));
        }

        public async Task<string> GenerateRefreshTokenAsync()
        {
            var secureRandomBytes = new byte[32];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            await Task.Run(() => randomNumberGenerator.GetBytes(secureRandomBytes));

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }

        #endregion
    }
}
