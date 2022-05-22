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

        /// <summary>
        /// Timeout for custom handler for <see cref="HttpClient" />. Used to prevent hanging.
        /// </summary>
        public TimeSpan HttpHandlerTimeout { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Common timeout for <see cref="HttpClient" />. It is the last priority timeout to use.
        /// </summary>
        public TimeSpan TimeoutOnRequest { get; set; } = TimeSpan.FromMinutes(2);

        public int RetryCountOnFailed { get; set; } = 3;

        /// <summary>
        /// Timeout on request failed for <see cref="HttpClient" />.
        /// </summary>
        public TimeSpan RetryTimeoutOnFailed { get; set; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Timeout on authentication failed for <see cref="HttpClient" />.
        /// </summary>
        /// <remarks>
        /// Consider how many retries. If auth lapses and you have valid credentials,
        /// one should be enough; too many tries can cause some auth systems to block or
        /// throttle the caller.
        /// </remarks>
        public int RetryCountOnAuth { get; set; } = 1;

        public TimeSpan RetryTimeoutOnAuth { get; set; } = TimeSpan.FromSeconds(1);

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
