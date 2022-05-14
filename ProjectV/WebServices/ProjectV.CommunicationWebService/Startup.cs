﻿using Acolyte.Assertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectV.CommonWebApi.Authorization.Passwords;
using ProjectV.CommonWebApi.Authorization.Tokens.Generators;
using ProjectV.CommonWebApi.Authorization.Tokens.Services;
using ProjectV.CommonWebApi.Authorization.Users.Services;
using ProjectV.CommonWebApi.Extensions;
using ProjectV.CommonWebApi.Models.Config;
using ProjectV.CommunicationWebService.v1.Domain.Configuration;
using ProjectV.CommunicationWebService.v1.Domain.Processing;
using ProjectV.Configuration.Options;
using ProjectV.Core.DependencyInjection;
using ProjectV.DataAccessLayer.Services.Tokens;
using ProjectV.DataAccessLayer.Services.Users;

namespace ProjectV.CommunicationWebService
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(
            IConfiguration configuration)
        {
            Configuration = configuration.ThrowIfNull(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceOptionsSection = Configuration.GetSection(nameof(ProjectVServiceOptions));
            var jwtConfigSecion = Configuration.GetSection(nameof(JwtConfiguration));
            var jwtConfig = jwtConfigSecion.Get<JwtConfiguration>();

            services.AddHttpClientWithOptions(serviceOptionsSection.Get<ProjectVServiceOptions>());
            services.AddSingleton<IConfigurationReceiver, ConfigurationReceiver>();
            services.AddSingleton<IProcessingResponseReceiver, ProcessingResponseReceiver>();

            services.AddTransient<IPasswordManager, PasswordManager>();
            services.AddSingleton<IUserInfoService, InMemoryUserInfoService>();
            services.AddSingleton<IRefreshTokenInfoService, InMemoryRefreshTokenInfoService>(
                provider => new InMemoryRefreshTokenInfoService(
                    jwtConfig.RefreshTokenExpirationTimeout,
                    provider.GetRequiredService<IUserInfoService>()
                )
            );
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddSingleton<IUserService, UserService>();

            services
                .Configure<ProjectVServiceOptions>(serviceOptionsSection)
                .Configure<JwtConfiguration>(jwtConfigSecion)
                .Configure<UserServiceOptions>(Configuration.GetSection(nameof(UserServiceOptions)));

            services
                .AddMvc(mvcOptions => mvcOptions.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            services.AddApiVersioningByNamespaceConvention();

            // Register the Swagger generator, defining 1 or more Swagger documents.
            services.ConfigureSwaggerGenWithOpenApi(
                title: "ProjectV Communication API",
                description: "Public Web API to make requests to ThingAppriser service.",
                apiVersion: "v1"
            );

            services.AddJtwAuthentication(jwtConfig);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request 
        // pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production 
                // scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "ProjectV Communication API v1");
                c.RoutePrefix = string.Empty;
            });

            app.ConfigureCustomExceptionMiddleware();
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
