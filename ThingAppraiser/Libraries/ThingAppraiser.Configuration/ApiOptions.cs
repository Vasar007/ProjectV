using Acolyte.Assertions;

namespace ThingAppraiser.Configuration
{
    public sealed class ApiOptions : IOptions
    {
        private string _tmdbApiKey = "TMDB_API_KEY";
        public string TmdbApiKey
        {
            get => EnvironmentVariablesParser.GetValueOrDefault("TmdbApiKey", _tmdbApiKey);
            set => _tmdbApiKey = value.ThrowIfNullOrWhiteSpace(nameof(value));
        }

        private string _omdbApiKey = "OMDB_API_KEY";
        public string OmdbApiKey
        {
            get => EnvironmentVariablesParser.GetValueOrDefault("OmdbApiKey", _omdbApiKey);
            set => _omdbApiKey = value.ThrowIfNullOrWhiteSpace(nameof(value));
        }

        private string _steamApiKey = "STEAM_API_KEY";
        public string SteamApiKey
        {
            get => EnvironmentVariablesParser.GetValueOrDefault("SteamApiKey", _steamApiKey);
            set => _steamApiKey = value.ThrowIfNullOrWhiteSpace(nameof(value));
        }


        public ApiOptions()
        {
        }
    }
}
