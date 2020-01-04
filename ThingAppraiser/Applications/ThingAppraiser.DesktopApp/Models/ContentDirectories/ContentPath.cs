using System.Windows.Input;
using Acolyte.Assertions;
using Prism.Commands;
using ThingAppraiser.DesktopApp.Domain;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentPath
    {
        public string Path { get; }

        public ICommand GoToPathCommand { get; }


        public ContentPath(string path)
        {
            Path = path.ThrowIfNullOrWhiteSpace(nameof(path));

            GoToPathCommand = new DelegateCommand(GoToLocalPath);
        }

        private void GoToLocalPath()
        {
            LinkOpener.OpenLocalFolder(Path);
        }
    }
}
