using System;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ProjectV.CommonWebApi.Extensions;
using ProjectV.TelegramBotWebService.v1.Domain;

namespace ProjectV.TelegramBotWebService
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration.ThrowIfNull(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IServiceSetup, ServiceSetup>();
            services.AddSingleton<IServiceProxy, ServiceProxy>();

            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<IBotService, BotService>();
            services.AddTransient<IUserCache, UserCache>();

            services.Configure<BotConfiguration>(Configuration.GetSection(nameof(BotConfiguration)));
            services.Configure<ServiceSettings>(Configuration.GetSection(nameof(ServiceSettings)));

            services
                .AddMvc(mvcOptions => mvcOptions.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

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

            // Register the Swagger generator, defining 1 or more Swagger documents.
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ProjectV TelegramBot API",
                    Description = "Web API to interact with Telegram Bot.",
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
                c.SwaggerEndpoint("./swagger/v1/swagger.json", "ProjectV TelegramBot API v1");
                c.RoutePrefix = string.Empty;
            });

            app.ConfigureCustomExceptionMiddleware();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
