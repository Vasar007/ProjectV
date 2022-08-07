using Acolyte.Assertions;

namespace ProjectV.Configuration.Options
{
    public sealed class ApiKeysOptions : IOptions
    {
        public string TmdbApiKey { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("TmdbApiKey", string.Empty);

        public string OmdbApiKey { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("OmdbApiKey", string.Empty);

        public string SteamApiKey { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("SteamApiKey", string.Empty);


        public ApiKeysOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            TmdbApiKey.ThrowIfNullOrWhiteSpace(nameof(TmdbApiKey));
            OmdbApiKey.ThrowIfNullOrWhiteSpace(nameof(OmdbApiKey));
            SteamApiKey.ThrowIfNullOrWhiteSpace(nameof(SteamApiKey));
        }

        #endregion
    }
}
