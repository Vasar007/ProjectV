using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class DialogHostProvider
    {
        private static readonly ILogger _logger =
           LoggerFactory.CreateLoggerFor(typeof(DialogHostProvider));


        public static async Task ShowDialog(object content, object dialogIdentifier,
           DialogClosingEventHandler closingEventHandler)
        {
            content.ThrowIfNull(nameof(content));
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            closingEventHandler.ThrowIfNull(nameof(closingEventHandler));

            object result = await DialogHost.Show(
                content, dialogIdentifier, closingEventHandler
            );

            _logger.Debug(
                $"Dialog was closed, the CommandParameter used to close it was: {result ?? "NULL"}."
            );
        }

        public static async Task ShowDialogExtended(object content, object dialogIdentifier,
            DialogOpenedEventHandler openedEventHandler,
            DialogClosingEventHandler closingEventHandler)
        {
            content.ThrowIfNull(nameof(content));
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            openedEventHandler.ThrowIfNull(nameof(openedEventHandler));
            closingEventHandler.ThrowIfNull(nameof(closingEventHandler));

            object result = await DialogHost.Show(
                content, dialogIdentifier, openedEventHandler, closingEventHandler
            );

            _logger.Debug(
                $"Dialog was closed, the CommandParameter used to close it was: {result ?? "NULL"}."
            );
        }
    }
}
