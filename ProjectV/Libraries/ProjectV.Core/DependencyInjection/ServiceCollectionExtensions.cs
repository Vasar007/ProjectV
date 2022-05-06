using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;

namespace ProjectV.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientWithOptions(
            this IServiceCollection services, ProjectVServiceOptions serviceOptions)
        {
            services.ThrowIfNull(nameof(services));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            services
                .AddHttpClient(serviceOptions.HttpClientDefaultName)
                .AddHttpOptions(serviceOptions);

            return services;
        }

        public static IServiceCollection AddHttpClientWithOptions<TClient, TImplementation>(
            this IServiceCollection services, ProjectVServiceOptions serviceOptions)
            where TClient : class
            where TImplementation : class, TClient
        {
            services.ThrowIfNull(nameof(services));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            services
                .AddHttpClient<TClient, TImplementation>(serviceOptions.HttpClientDefaultName)
                .AddHttpOptions(serviceOptions);

            return services;
        }
    }
}
