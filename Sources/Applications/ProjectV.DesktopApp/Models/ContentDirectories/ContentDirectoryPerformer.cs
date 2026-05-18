using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.DesktopApp.Domain.Executor;

namespace ProjectV.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentDirectoryPerformer :
        IPerformer<ContentDirectoryParametersInfo, ContentDirectoryInfo>
    {
        private readonly ContentFinderWrapper _contentFinder;


        public ContentDirectoryPerformer()
        {
            _contentFinder = new ContentFinderWrapper();
        }

        #region IPerformer<ContentDirectoryParametersInfo, ContentDirectoryInfo> Implementation

        public async Task<ContentDirectoryInfo> PerformAsync(
            ContentDirectoryParametersInfo parameters)
        {
            parameters.ThrowIfNull(nameof(parameters));

            ContentDirectoryInfo result = await _contentFinder
                .GetAllDirectoryContentAsync(parameters.DirectoryPath, parameters.ContentType)
                .ConfigureAwait(false);

            return result;
        }

        #endregion
    }
}
