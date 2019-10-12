using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp
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

            _logger.PrintHeader("Desktop client application started.");
        }

        protected override Window CreateShell()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            
            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ToplistHeader, typeof(ToplistHeaderView)
            );

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ToplistEditor, typeof(ToplistEditorView)
            );

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ContentFinderHeader, typeof(ContentFinderHeaderView)
            );

            regionManager.RegisterViewWithRegion(
                DesktopOptions.BindingNames.ContentFinderResults, typeof(ContentFinderResultsView)
            );

            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // TODO: register common domain and models classes here.
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // TODO: split desktop project on separate modules.
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _logger.PrintFooter("Desktop client application stopped.");
        }
    }
}
