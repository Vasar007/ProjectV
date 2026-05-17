using System;
using System.Net.Http;
using Acolyte.Assertions;
using Acolyte.Common.Monads;
using MihaZupan;
using ProjectV.Configuration.Options;
using ProjectV.Logging;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientHandlerExtensions
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(HttpClientHandlerExtensions));


        public static HttpClientHandler ConfigureHandlerWithOptions(
            this HttpClientOptions options)
        {
            options.ThrowIfNull(nameof(options));

            return new HttpClientHandler()
                .ConfigureHandlerWithOptions(options);
        }

        public static HttpClientHandler ConfigureHandlerWithOptions(
            this HttpClientHandler handler, HttpClientOptions options)
        {
            handler.ThrowIfNull(nameof(handler));
            options.ThrowIfNull(nameof(options));

            handler.AllowAutoRedirect = options.AllowAutoRedirect;
            handler.UseCookies = options.UseCookies;
            handler.UseProxy = options.UseDefaultProxy;

            return handler
                .ApplyIf(!options.ValidateServerCertificates, h => h.DisableServerCertificateValidation())
                .ApplyIf(options.UseSocks5Proxy, h => h.ConfigureSocks5Proxy(options));
        }

        public static HttpClientHandler DisableServerCertificateValidation(
            this HttpClientHandler handler)
        {
            handler.ThrowIfNull(nameof(handler));

            _logger.Warn("ATTENTION! Server certificates validation is disabled.");
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            return handler;
        }

        public static HttpClientHandler ConfigureSocks5Proxy(this HttpClientHandler handler,
            HttpClientOptions options)
        {
            handler.ThrowIfNull(nameof(handler));
            options.ThrowIfNull(nameof(options));

            var socks5HostName = options.Socks5HostName;
            var socks5Port = options.Socks5Port;
            if (string.IsNullOrWhiteSpace(socks5HostName) ||
                socks5Port is null)
            {
                const string message = "Failed to configure SOCKS5 proxy: specify valid options.";
                throw new ArgumentException(message, nameof(options));
            }

            _logger.Info($"Using SOCKS5 proxy [{socks5HostName}, {socks5Port}].");

            handler.UseProxy = true;
            handler.Proxy = new HttpToSocks5Proxy(
                socks5HostName, socks5Port.Value
            );

            return handler;
        }
    }
}
