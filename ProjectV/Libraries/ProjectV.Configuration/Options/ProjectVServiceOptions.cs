using System;
using ProjectV.Options;

namespace ProjectV.Configuration.Options
{
    public sealed class ProjectVServiceOptions : IOptions
    {
        public RestApiOptions RestApi { get; set; } = default!;

        public string HttpClientDefaultName { get; set; } = CommonConstants.ApplicationName;

        // It is common practice to not dispose HttpClient.
        public bool ShouldDisposeHttpClient { get; set; } = false;

        public TimeSpan HttpHandlerTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan HttpClientTimeoutOnRequest { get; set; } = TimeSpan.FromMinutes(2);

        public int HttpClientRetryCountOnFailed { get; set; } = 3;

        public TimeSpan HttpClientRetryTimeoutOnFailed { get; set; } = TimeSpan.FromSeconds(2);

        // Consider how many retries. If auth lapses and you have valid credentials,
        // one should be enough; too many tries can cause some auth systems to block or
        // throttle the caller. 
        public int HttpClientRetryCountOnAuth { get; set; } = 1;

        public TimeSpan HttpClientRetryTimeoutOnAuth { get; set; } = TimeSpan.FromSeconds(1);


        public ProjectVServiceOptions()
        {
        }
    }
}
