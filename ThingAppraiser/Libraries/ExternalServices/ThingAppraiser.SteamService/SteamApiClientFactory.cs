namespace ThingAppraiser.SteamService
{
    public static class SteamApiClientFactory
    {
        public static ISteamApiClient CreateClient(string apiKey)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            return new SteamApiClient(apiKey);
        }
    }
}
