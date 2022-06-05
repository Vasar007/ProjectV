using System;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectV.CommonWebApi.Models.Options;
using ProjectV.CommonWebApi.Service.Hosted;

namespace ProjectV.CommonWebApi.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJtwAuthentication(this IServiceCollection services,
            JwtOptions jwtOptions)
        {
            services.ThrowIfNull(nameof(services));
            jwtOptions.ThrowIfNull(nameof(jwtOptions));

            if (string.IsNullOrWhiteSpace(jwtOptions.SecretKey))
            {
                throw new ArgumentException("Config does not have secret key.", nameof(jwtOptions));
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(bearerOptions =>
                {
                    var key = new SymmetricSecurityKey(
                        Convert.FromBase64String(jwtOptions.SecretKey)
                    );
                    bearerOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection ConfigureSwaggerGenWithOpenApi(
            this IServiceCollection services, string title, string description, string apiVersion)
        {
            services.ThrowIfNull(nameof(services));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(apiVersion, new OpenApiInfo
                {
                    Version = apiVersion,
                    Title = title,
                    Description = description,
                    //TermsOfService = "None",
                    Contact = new OpenApiContact
                    {
                        Name = "Vasily Vasilyev",
                        Email = "vasar007@yandex.ru",
                        Url = new Uri("https://t.me/Vasar007")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Apache License 2.0",
                        Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0")
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddApiVersioningByNamespaceConvention(
            this IServiceCollection services)
        {
            services.ThrowIfNull(nameof(services));

            services.AddApiVersioning(
               options =>
               {
                   // Reporting api versions will return the headers "api-supported-versions" and 
                   // "api-deprecated-versions".
                   options.ReportApiVersions = true;

                   // Automatically applies an api version based on the name of the defining 
                   // controller's namespace.
                   options.Conventions.Add(new VersionByNamespaceConvention());
               }
           );

            return services;
        }

        public static IServiceCollection AddHostedService<THostedService>(
            this IServiceCollection services,
            Func<IServiceProvider, THostedService> implementationFactory,
            bool ignoreServiceSetupErrors)
            where THostedService : class, IHostedService
        {
            services.ThrowIfNull(nameof(services));
            implementationFactory.ThrowIfNull(nameof(implementationFactory));

            if (ignoreServiceSetupErrors)
            {
                return services.AddHostedServiceAsSafe(implementationFactory);
            }

            return services.AddHostedService(implementationFactory);
        }

        public static IServiceCollection AddHostedServiceAsSafe<THostedService>(
            this IServiceCollection services,
            Func<IServiceProvider, THostedService> implementationFactory)
            where THostedService : class, IHostedService
        {
            services.ThrowIfNull(nameof(services));
            implementationFactory.ThrowIfNull(nameof(implementationFactory));

            return services.AddHostedService(provider =>
            {
                var realHostedService = implementationFactory(provider);
                return new HostedServiceSafeWrapper(realHostedService);
            });
        }
    }
}
