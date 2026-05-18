using Acolyte.Assertions;

namespace ProjectV.OmdbService
{
    public static class OmdbClientFactory
    {
        public static IOmdbClient CreateClient(string apiKey)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            return new OmdbClient(apiKey);
        }
    }
}
