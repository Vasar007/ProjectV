using System;

namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        public string CommunicationServiceBaseAddress { get; set; } = default!;

        public string CommunicationServiceRequestApiUrl { get; set; } = default!;

        public string CommunicationServiceLoginApiUrl { get; set; } = default!;

        public string? AccessToken { get; set; } =
            EnvironmentVariablesParser.GetValueOrDefault("AccessToken", string.Empty);

        // It is common practice to not dispose HttpClient.
        public bool DisposeHttpClient { get; set; } = false;

        public int HttpClientRetryCount { get; set; } = 3;
        public TimeSpan HttpClientRetryTimeout { get; set; } = TimeSpan.FromSeconds(2);


        public ProjectVServiceOptions()
        {
        }
    }
}
