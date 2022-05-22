using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using ProjectV.Options;

namespace ProjectV.Configuration.Options
{
    public sealed class HttpClientOptions : IOptions
    {
        public string HttpClientDefaultName { get; set; } = CommonConstants.ApplicationName;

        /// <summary>
        /// It is common practice to not dispose <see cref="HttpClient" />.
        /// </summary>
        public bool ShouldDisposeHttpClient { get; set; } = false;

        public TimeSpan HttpHandlerTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan HttpClientTimeoutOnRequest { get; set; } = TimeSpan.FromMinutes(2);

        public int HttpClientRetryCountOnFailed { get; set; } = 3;

        public TimeSpan HttpClientRetryTimeoutOnFailed { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Consider how many retries. If auth lapses and you have valid credentials,
        /// one should be enough; too many tries can cause some auth systems to block or
        /// throttle the caller.
        /// </summary>
        public int HttpClientRetryCountOnAuth { get; set; } = 1;

        public TimeSpan HttpClientRetryTimeoutOnAuth { get; set; } = TimeSpan.FromSeconds(1);

        public bool ValidateServerCertificates { get; set; } = true;

        public bool AllowAutoRedirect { get; set; } = true;

        public bool UseCookies { get; set; } = true;

        public bool UseDefaultProxy { get; set; } = true;

        public bool UseSocks5Proxy { get; set; } = false;

        public string? Socks5HostName { get; set; }

        [Range(0, 65535, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Socks5Port { get; set; }


        public HttpClientOptions()
        {
        }
    }
}
