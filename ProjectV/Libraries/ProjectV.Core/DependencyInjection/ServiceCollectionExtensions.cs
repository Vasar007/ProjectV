using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;

namespace ProjectV.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddHttpClientWithOptions(
            this IServiceCollection services, HttpClientOptions options)
        {
            services.ThrowIfNull(nameof(services));
            options.ThrowIfNull(nameof(options));

            return services
                .AddHttpClient(options.HttpClientDefaultName)
                .AddBuilderOptionsInternal(options);
        }

        public static IHttpClientBuilder AddHttpClientWithOptions<TClient>(
           this IServiceCollection services, HttpClientOptions options)
           where TClient : class
        {
            services.ThrowIfNull(nameof(services));
            options.ThrowIfNull(nameof(options));

            return services
                .AddHttpClient<TClient>(options.HttpClientDefaultName)
                .AddBuilderOptionsInternal(options);
        }

        public static IHttpClientBuilder AddHttpClientWithOptions<TClient, TImplementation>(
            this IServiceCollection services, HttpClientOptions options)
            where TClient : class
            where TImplementation : class, TClient
        {
            services.ThrowIfNull(nameof(services));
            options.ThrowIfNull(nameof(options));

            return services
                .AddHttpClient<TClient, TImplementation>(options.HttpClientDefaultName)
                .AddBuilderOptionsInternal(options);
        }

        private static IHttpClientBuilder AddBuilderOptionsInternal(this IHttpClientBuilder builder,
            HttpClientOptions options)
        {
            return builder
                .AddHttpOptions(options);
        }
    }
}
