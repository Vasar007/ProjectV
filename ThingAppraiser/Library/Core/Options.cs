namespace ThingAppraiser.Core
{
    public static class Options
    {
        private static readonly string _tmdbApiKey = "TMDB_API_KEY";

        private static readonly string _omdbApiKey = "OMDB_API_KEY";

        private static readonly string _steamApiKey = "STEAM_API_KEY";

        public static string TmdbApiKey { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("TmdbApiKey", _tmdbApiKey);

        public static string OmdbApiKey { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("OmdbApiKey", _omdbApiKey);

        public static string SteamApiKey { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("SteamApiKey", _steamApiKey);
    }
}
