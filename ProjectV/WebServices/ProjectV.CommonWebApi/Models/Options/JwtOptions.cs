using System;
using System.ComponentModel.DataAnnotations;
using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.CommonWebApi.Models.Options
{
    public sealed class JwtOptions : IOptions
    {
        public string SecretKey { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("JwtSecretKey", string.Empty);

        [Required(AllowEmptyStrings = false)]
        public string Issuer { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string Audience { get; init; } = default!;

        public TimeSpan AccessTokenExpirationTimeout { get; init; } = TimeSpan.FromHours(3);
        public TimeSpan RefreshTokenExpirationTimeout { get; init; } = TimeSpan.FromDays(7);


        public JwtOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            SecretKey.ThrowIfNullOrWhiteSpace(nameof(SecretKey));
            Issuer.ThrowIfNullOrWhiteSpace(nameof(Issuer));
            Audience.ThrowIfNullOrWhiteSpace(nameof(Audience));
        }

        #endregion
    }
}
