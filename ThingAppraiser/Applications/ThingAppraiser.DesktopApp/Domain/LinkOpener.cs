using System.Diagnostics;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class LinkOpener
    {
        public static void OpenLocalFolder(string path)
        {
            path.ThrowIfNullOrWhiteSpace(nameof(path));

            // This Windows-only solution to open folder and select a file by path.
            Process.Start(new ProcessStartInfo("explorer.exe", $" /select, {path}"));
        }
    }
}
