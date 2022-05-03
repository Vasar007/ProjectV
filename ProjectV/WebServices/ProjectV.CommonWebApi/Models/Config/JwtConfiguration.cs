namespace ProjectV.CommonWebApi.Models.Config
{
    // TODO: make this DTO immutable.
    public sealed class JwtConfiguration
    {
        // Not use "string.Empty" to simplify insertion of hard-coded value.
        private static readonly string _defaultSecretKey = "";

        public string SecretKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("JwtSecretKey", _defaultSecretKey);

        public string Issuer { get; set; } = default!;

        public string Audience { get; set; } = default!;


        public JwtConfiguration()
        {
        }
    }
}
