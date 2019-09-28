using ThingAppraiser.Extensions;

namespace ThingAppraiser.TmdbService
{
    public static class TmdbClientFactory
    {
        public static ITmdbClient CreateClient(string apiKey, int maxRetryCount)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            return new TmdbClient(apiKey)
            {
                MaxRetryCount = maxRetryCount
            };
        }
    }
}
