using Acolyte.Assertions;

namespace ThingAppraiser.TmdbService
{
    public static class TmdbClientFactory
    {
        public static ITmdbClient CreateClient(string apiKey, int maxRetryCount)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));
            maxRetryCount.ThrowIfValueIsOutOfRange(nameof(maxRetryCount), 0, int.MaxValue);

            return new TmdbClient(apiKey)
            {
                MaxRetryCount = maxRetryCount
            };
        }
    }
}
