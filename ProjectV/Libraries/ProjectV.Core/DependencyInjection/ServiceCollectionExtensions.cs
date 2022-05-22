using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;

namespace ProjectV.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientWithOptions(
            this IServiceCollection services, HttpClientOptions options)
        {
            services.ThrowIfNull(nameof(services));
            options.ThrowIfNull(nameof(options));

            services
                .AddHttpClient(options.HttpClientDefaultName)
                .AddBuilderOptionsInternal(options);

            return services;
        }

        public static IServiceCollection AddHttpClientWithOptions<TClient>(
           this IServiceCollection services, HttpClientOptions options)
           where TClient : class
        {
            services.ThrowIfNull(nameof(services));
            options.ThrowIfNull(nameof(options));

            services
                .AddHttpClient<TClient>(options.HttpClientDefaultName)
                .AddBuilderOptionsInternal(options);

            return services;
        }

        public static IServiceCollection AddHttpClientWithOptions<TClient, TImplementation>(
            this IServiceCollection services, HttpClientOptions options)
            where TClient : class
            where TImplementation : class, TClient
        {
            services.ThrowIfNull(nameof(services));
            options.ThrowIfNull(nameof(options));

            services
                .AddHttpClient<TClient, TImplementation>(options.HttpClientDefaultName)
                .AddBuilderOptionsInternal(options);

            return services;
        }

        private static IHttpClientBuilder AddBuilderOptionsInternal(this IHttpClientBuilder builder,
            HttpClientOptions options)
        {
            builder
                .AddHttpOptions(options);

            return builder;
        }
    }
}
