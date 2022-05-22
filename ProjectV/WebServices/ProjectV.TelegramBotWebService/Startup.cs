using Acolyte.Assertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectV.CommonWebApi.Extensions;
using ProjectV.CommonWebApi.Models.Options;
using ProjectV.Configuration.Options;
using ProjectV.Core.DependencyInjection;
using ProjectV.Core.Services.Clients;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.TelegramBotWebService.v1.Domain.Cache;
using ProjectV.TelegramBotWebService.v1.Domain.Handlers;
using ProjectV.TelegramBotWebService.v1.Domain.Receivers;
using ProjectV.TelegramBotWebService.v1.Domain.Setup;
using ProjectV.TelegramBotWebService.v1.Domain.Text;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService
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

            services.AddSingleton<IServiceSetup, ServiceSetup>();
            services.AddHttpClientWithOptions(serviceOptionsSection.Get<ProjectVServiceOptions>().HttpClient);
            services.AddTransient<ICommunicationServiceClient, CommunicationServiceClient>();

            services.AddSingleton<IUpdateService, UpdateService>();
            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<IBotHandler<Message>, BotMessageHandler>();
            services.AddTransient<IUserCache, UserCache>();
            services.AddTransient<ITelegramTextProcessor, TelegramTextProcessor>();
            services.AddTransient<IProcessingResponseReceiver, ProcessingResponseReceiver>();

            var jwtConfigSecion = Configuration.GetSection(nameof(JwtOptions));
            services
                .Configure<ProjectVServiceOptions>(serviceOptionsSection)
                .Configure<JwtOptions>(jwtConfigSecion)
                .Configure<TelegramBotWebServiceOptions>(Configuration.GetSection(nameof(TelegramBotWebServiceOptions)));

            services
                .AddMvc(mvcOptions => mvcOptions.EnableEndpointRouting = false)
                .AddNewtonsoftJson();

            services.AddApiVersioningByNamespaceConvention();

            // Register the Swagger generator, defining 1 or more Swagger documents.
            services.ConfigureSwaggerGenWithOpenApi(
               title: "ProjectV TelegramBot API",
               description: "Web API to interact with Telegram Bot.",
               apiVersion: "v1"
           );

            services.AddJtwAuthentication(jwtConfigSecion.Get<JwtOptions>());
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
