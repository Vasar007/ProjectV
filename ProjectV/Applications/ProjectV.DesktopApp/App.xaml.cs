using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using ProjectV.Configuration;
using ProjectV.Core.DependencyInjection;
using ProjectV.Core.Proxies;
using ProjectV.DesktopApp.Domain;
using ProjectV.DesktopApp.Views;
using ProjectV.Logging;

namespace ProjectV.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : PrismApplication
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<App>();


        public App()
        {
            // Set current culture for app globally.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.InvariantCulture.Name)
                )
            );

            UnhandledExceptionEventRegistrator.Register(Application_OnUnhandledException);

            _logger.PrintHeader("Desktop client application started.");
        }

        protected override Window CreateShell()
        {
            var regionManager = Container.Resolve<IRegionManager>();

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ToplistHeader,
                typeof(ToplistHeaderView)
            );

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ToplistEditor,
                typeof(ToplistEditorView)
            );

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ContentDirectoriesHeader,
                typeof(ContentDirectoriesHeaderView)
            );

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ContentDirectoriesResults,
                typeof(ContentDirectoriesResultsView)
            );

            return Container.Resolve<MainWindow>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClientWithOptions(ConfigOptions.ProjectVService);
        }

        private static IServiceProvider CreateServices()
        {
            var host = new HostBuilder()
                .ConfigureServices(services => ConfigureServices(services))
                .Build();

            return host.Services;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var services = CreateServices();

            // TODO: register common domain and models classes here.
            containerRegistry.Register<IHttpClientFactory>(() => services.GetRequiredService<IHttpClientFactory>());
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // TODO: split desktop project on separate modules.
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _logger.PrintFooter("Desktop client application stopped.");
        }

        private void Application_OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            _logger.Error(ex, "Unhandled exception has been occurred.");
        }
    }
}
