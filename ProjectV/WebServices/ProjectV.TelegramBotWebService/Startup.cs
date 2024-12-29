using Acolyte.Assertions;
using Acolyte.Common.Monads;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProjectV.CommonWebApi.Controllers.Extensions;
using ProjectV.CommonWebApi.Middleware.Extensions;
using ProjectV.CommonWebApi.Models.Options;
using ProjectV.CommonWebApi.Service.Extensions;
using ProjectV.CommonWebApi.Service.Setup;
using ProjectV.CommonWebApi.Service.Setup.Factories;
using ProjectV.Configuration.Options;
using ProjectV.Core.DependencyInjection;
using ProjectV.Core.Services.Clients;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Controllers;
using ProjectV.TelegramBotWebService.v1.Domain;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.TelegramBotWebService.v1.Domain.Handlers;
using ProjectV.TelegramBotWebService.v1.Domain.Polling;
using ProjectV.TelegramBotWebService.v1.Domain.Polling.Factories;
using ProjectV.TelegramBotWebService.v1.Domain.Polling.Handlers;
using ProjectV.TelegramBotWebService.v1.Domain.Receivers;
using ProjectV.TelegramBotWebService.v1.Domain.Service.Setup.Factories;
using ProjectV.TelegramBotWebService.v1.Domain.Services.Hosted;
using ProjectV.TelegramBotWebService.v1.Domain.Text;
using ProjectV.TelegramBotWebService.v1.Domain.Users.Cache;
using ProjectV.TelegramBotWebService.v1.Domain.Webhooks;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService
{
    public sealed class Startup
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<Startup>();

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

            services.AddSingleton<IServiceSetupActionsFactory, TelegramBotWebServiceSetupActionsFactory>();
            services.AddSingleton<IServiceSetup, ServiceSetup>();
            services.AddHttpClientWithOptions(serviceOptionsSection.GetChecked<ProjectVServiceOptions>().HttpClient);
            services.AddTransient<ICommunicationServiceClient, CommunicationServiceClient>();

            services.AddSingleton<IBotService, BotService>();
            services.AddSingleton<ITelegramUserCache, TelegramUserCache>();
            services.AddTransient<ITelegramTextProcessor, TelegramTextProcessor>();
            services.AddTransient<IProcessingResponseReceiver, ProcessingResponseReceiver>();
            services.AddSingleton<IBotHandler<Message>, BotMessageHandler>();

            services.AddSingleton<IUpdateService, UpdateService>();

            services.AddSingleton<IBotWebhook, BotWebhook>();

            services.AddSingleton<IBotPollingUpdateHandler, BotPollingUpdateHandler>();
            services.AddSingleton<IBotPollingReceiverFactory, BotPollingReceiverFactory>();
            services.AddSingleton<IBotPolling, BotPolling>();

            var jwtOptionsSecion = Configuration.GetSection(nameof(JwtOptions));
            var botWebServiceSecion = Configuration.GetSection(nameof(TelegramBotWebServiceOptions));
            services
                .Configure<ProjectVServiceOptions>(serviceOptionsSection)
                .Configure<JwtOptions>(jwtOptionsSecion)
                .Configure<TelegramBotWebServiceOptions>(botWebServiceSecion)
                .Configure<UserServiceOptions>(Configuration.GetSection(nameof(UserServiceOptions)));

            // Add hosted service as background activities depending on the set working mode.
            var botWebServiceOptions = botWebServiceSecion.GetChecked<TelegramBotWebServiceOptions>();
            services
                .ApplyIf(
                    botWebServiceOptions.IsMode(TelegramBotWebServiceWorkingMode.WebhookViaHostedService),
                    x => x.AddHostedService(ConfigureWebhook.Create, botWebServiceOptions.IgnoreServiceSetupErrors)
                )
                .ApplyIf(
                    botWebServiceOptions.IsMode(TelegramBotWebServiceWorkingMode.PollingViaHostedService),
                    x => x.AddHostedService(PoolingProcessor.Create, botWebServiceOptions.IgnoreServiceSetupErrors)
                );

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

            services.AddJtwAuthentication(jwtOptionsSecion.GetChecked<JwtOptions>());
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
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                ConfigureCustomBotEndpoint(app, endpoints);
                endpoints.MapControllers();
            });
        }

        private static void ConfigureCustomBotEndpoint(IApplicationBuilder app,
            IEndpointRouteBuilder endpoints)
        {
            // See note in TelegramBotWebServiceOptions.ConstructWebhookUrlWithBotToken method.
            var wrappedOptions = app.ApplicationServices
                .GetRequiredService<IOptions<TelegramBotWebServiceOptions>>();
            var botOptions = wrappedOptions.Value;
            if (!botOptions.Bot.Webhook.UseBotTokenInUrl)
            {
                _logger.Info("Using default mapping for controllers.");
                return;
            }

            var pattern = botOptions.GetServiceApiUrl();
            var controller = ControllerExtensions.GetControllerNameFromType<UpdateController>();
            _logger.Info($"Configuring custom endpoint for {controller} controller: [{pattern}].");

            endpoints.MapControllerRoute(
                name: "tgwebhook",
                pattern: pattern,
                new
                {
                    controller,
                    action = nameof(UpdateController.Post)
                }
            );
        }
    }
}
