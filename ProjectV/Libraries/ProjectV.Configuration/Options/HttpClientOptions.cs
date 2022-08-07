﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using Acolyte.Assertions;
using ProjectV.Options;

namespace ProjectV.Configuration.Options
{
    public sealed class HttpClientOptions : IOptions
    {
        public string HttpClientDefaultName { get; init; } = CommonConstants.ApplicationName;

        /// <summary>
        /// It is common practice to not dispose <see cref="HttpClient" />.
        /// </summary>
        public bool ShouldDisposeHttpClient { get; init; } = false;

        /// <summary>
        /// Timeout for custom handler for <see cref="HttpClient" />. Used to prevent hanging.
        /// </summary>
        public TimeSpan HttpHandlerTimeout { get; init; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Common timeout for <see cref="HttpClient" />. It is the last priority timeout to use.
        /// </summary>
        public TimeSpan TimeoutOnRequest { get; init; } = TimeSpan.FromMinutes(2);

        public int RetryCountOnFailed { get; init; } = 3;

        /// <summary>
        /// Timeout on request failed for <see cref="HttpClient" />.
        /// </summary>
        public TimeSpan RetryTimeoutOnFailed { get; init; } = TimeSpan.FromSeconds(2);

        /// <summary>
        /// Timeout on authentication failed for <see cref="HttpClient" />.
        /// </summary>
        /// <remarks>
        /// Consider how many retries. If auth lapses and you have valid credentials,
        /// one should be enough; too many tries can cause some auth systems to block or
        /// throttle the caller.
        /// </remarks>
        public int RetryCountOnAuth { get; init; } = 1;

        public TimeSpan RetryTimeoutOnAuth { get; init; } = TimeSpan.FromSeconds(1);

        public bool ValidateServerCertificates { get; init; } = true;

        public bool AllowAutoRedirect { get; init; } = true;

        public bool UseCookies { get; init; } = true;

        public bool UseDefaultProxy { get; init; } = true;

        public bool UseSocks5Proxy { get; init; } = false;

        public string? Socks5HostName { get; init; }

        [Range(CommonConstants.MinPort, CommonConstants.MaxPort, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int? Socks5Port { get; init; }


        public HttpClientOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            HttpClientDefaultName.ThrowIfNullOrWhiteSpace(nameof(HttpClientDefaultName));
            RetryCountOnFailed.ThrowIfValueIsOutOfRange(nameof(RetryCountOnFailed), 0, int.MaxValue);
            RetryCountOnAuth.ThrowIfValueIsOutOfRange(nameof(RetryCountOnAuth), 0, int.MaxValue);

            if (UseSocks5Proxy)
            {
                Socks5HostName.ThrowIfNullOrWhiteSpace(nameof(Socks5HostName));
                Socks5Port.ThrowIfNullValue(nameof(Socks5Port));
                Socks5Port.GetValueOrDefault().ThrowIfValueIsOutOfRange(nameof(Socks5Port), CommonConstants.MinPort, CommonConstants.MaxPort);
            }
        }

        #endregion
    }
}
