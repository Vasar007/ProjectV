using System.Windows;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class MessageBoxHelper
    {
        public static MessageBoxResult ShowInfo(string messageText, string caption)
        {
            messageText.ThrowIfNull(nameof(messageText));
            caption.ThrowIfNull(nameof(caption));

            return MessageBox.Show(
                messageText, caption, MessageBoxButton.OK, MessageBoxImage.Information
            );
        }

        public static MessageBoxResult ShowInfo(string messageText)
        {
            return ShowInfo(messageText, caption: DesktopOptions.Title);
        }

        public static MessageBoxResult ShowError(string messageText, string caption)
        {
            messageText.ThrowIfNull(nameof(messageText));
            caption.ThrowIfNull(nameof(caption));

            return MessageBox.Show(
                messageText, caption, MessageBoxButton.OK, MessageBoxImage.Error
            );
        }

        public static MessageBoxResult ShowError(string messageText)
        {
            return ShowError(messageText, caption: DesktopOptions.Title);
        }
    }
}
