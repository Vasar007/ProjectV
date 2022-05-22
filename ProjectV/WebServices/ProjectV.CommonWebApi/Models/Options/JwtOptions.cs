using System;
using System.ComponentModel.DataAnnotations;
using ProjectV.Configuration;

namespace ProjectV.CommonWebApi.Models.Options
{
    public sealed class JwtOptions : IOptions
    {
        public string SecretKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("JwtSecretKey", string.Empty);

        [Required(AllowEmptyStrings = false)]
        public string Issuer { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string Audience { get; set; } = default!;

        public TimeSpan AccessTokenExpirationTimeout { get; set; } = TimeSpan.FromHours(3);
        public TimeSpan RefreshTokenExpirationTimeout { get; set; } = TimeSpan.FromDays(7);


        public JwtOptions()
        {
        }
    }
}
