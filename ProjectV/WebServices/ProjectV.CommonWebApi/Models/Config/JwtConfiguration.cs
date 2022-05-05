using System;

namespace ProjectV.CommonWebApi.Models.Config
{
    // TODO: make this DTO immutable.
    public sealed class JwtConfiguration
    {
        public string SecretKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("JwtSecretKey", string.Empty);

        public string Issuer { get; set; } = default!;

        public string Audience { get; set; } = default!;

        public TimeSpan ClaimExpirationTimeout { get; set; } = TimeSpan.FromHours(3);


        public JwtConfiguration()
        {
        }
    }
}
