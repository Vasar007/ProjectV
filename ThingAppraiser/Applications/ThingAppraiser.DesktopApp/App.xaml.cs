using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
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
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
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
