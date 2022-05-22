using System;
using ProjectV.Configuration;

namespace ProjectV.CommonWebApi.Models.Options
{
    // TODO: make this DTO immutable.
    public sealed class JwtOptions : IOptions
    {
        public string SecretKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("JwtSecretKey", string.Empty);

        public string Issuer { get; set; } = default!;

        public string Audience { get; set; } = default!;

        public TimeSpan AccessTokenExpirationTimeout { get; set; } = TimeSpan.FromHours(3);
        public TimeSpan RefreshTokenExpirationTimeout { get; set; } = TimeSpan.FromDays(7);


        public JwtOptions()
        {
        }
    }
}
