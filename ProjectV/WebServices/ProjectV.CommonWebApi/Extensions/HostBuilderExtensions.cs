using System;
using System.Runtime.InteropServices;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ProjectV.CommonWebApi.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseServiceDependOnOSPlatform(this IHostBuilder hostBuilder)
        {
            hostBuilder.ThrowIfNull(nameof(hostBuilder));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return hostBuilder.UseWindowsService();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return hostBuilder.UseSystemd();
            }

            string message = $"OS {RuntimeInformation.OSDescription} is not supported.";
            throw new NotSupportedException(message);
        }
    }
}
