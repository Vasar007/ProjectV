using System;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectV.CommonWebApi.Models.Config;

namespace ProjectV.CommonWebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJtwAuthentication(this IServiceCollection services,
            JwtConfiguration config)
        {
            services.ThrowIfNull(nameof(services));
            config.ThrowIfNull(nameof(config));

            if (string.IsNullOrWhiteSpace(config.SecretKey))
            {
                throw new ArgumentException("Config does not have secret key.", nameof(config));
            }

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = new SymmetricSecurityKey(Convert.FromBase64String(config.SecretKey));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = config.Issuer,
                        ValidAudience = config.Audience,
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
    }
}
