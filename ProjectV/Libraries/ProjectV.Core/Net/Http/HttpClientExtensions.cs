using System.Net.Http;
using Acolyte.Assertions;
using ProjectV.Configuration.Options;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientExtensions
    {
        public static bool DisposeClient(this HttpClient? client,
            ProjectVServiceOptions serviceOptions)
        {
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            if (serviceOptions.DisposeHttpClient)
            {
                client?.Dispose();
                return true;
            }

            return false;
        }
    }
}
