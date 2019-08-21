using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App : Application
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<App>();


        public App()
        {
            // Set current culture for app globally.
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)
                )
            );

            _logger.PrintHeader("Desktop client application started.");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _logger.PrintFooter("Desktop client application stopped.");
        }
    }
}
