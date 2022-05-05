using System;
using System.Net.Http;
using Acolyte.Assertions;

namespace ProjectV.Core.Net.Http
{
    public static class HttpRequestExtensions
    {
        private static readonly string TimeoutPropertyKey = "RequestTimeout";
        private static HttpRequestOptionsKey<TimeSpan?> TimeoutPropertyStrongKey => new(TimeoutPropertyKey);

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            request.ThrowIfNull(nameof(request));

            request.Options.Set(TimeoutPropertyStrongKey, timeout);
        }

        public static TimeSpan? FindTimeout(this HttpRequestMessage request)
        {
            request.ThrowIfNull(nameof(request));

            if (request.Options.TryGetValue(TimeoutPropertyStrongKey, out var timeout))
            {
                return timeout;
            }

            return null;
        }
    }
}
