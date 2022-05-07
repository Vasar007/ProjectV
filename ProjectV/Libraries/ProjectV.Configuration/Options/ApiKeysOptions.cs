namespace ProjectV.Configuration.Options
{
    public sealed class ApiKeysOptions : IOptions
    {
        public string TmdbApiKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("TmdbApiKey", string.Empty);

        public string OmdbApiKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("OmdbApiKey", string.Empty);

        public string SteamApiKey { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("SteamApiKey", string.Empty);


        public ApiKeysOptions()
        {
        }
    }
}
