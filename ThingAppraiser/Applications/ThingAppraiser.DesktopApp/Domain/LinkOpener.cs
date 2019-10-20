using System.Diagnostics;
using System.IO;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class LinkOpener
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(LinkOpener));

        public static void OpenLocalFolder(string path)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            if (!File.Exists(path))
            {
                _logger.Warn($"Specified path \"{path}\" is invalid: file is not exist.");
                return;
            }

            // This Windows-only solution to open folder and select a file by path.
            const string explorerFilename = "explorer.exe";
            string args = $" /select, \"{path}\""; // To proper handle file selecting.
            Process.Start(new ProcessStartInfo(explorerFilename, args));
        }
    }
}
