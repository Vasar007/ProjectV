using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;

namespace ProjectV.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpClientWithOptions(this IServiceCollection services,
            ProjectVServiceOptions serviceOptions)
        {
            services.ThrowIfNull(nameof(services));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            services
                .AddHttpClient(serviceOptions.HttpClientDefaultName)
                .AddTransientHttpErrorPolicyWithOptions(serviceOptions);

            return services;
        }
    }
}
