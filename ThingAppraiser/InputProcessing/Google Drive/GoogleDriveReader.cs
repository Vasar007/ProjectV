using System;
using System.IO;
using System.Collections.Generic;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using GoogleDriveData = Google.Apis.Drive.v3.Data;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Concrete implementation of reader part for Google Drive API.
    /// </summary>
    public class CGoogleDriveReader : CGoogleDriveWorker, IInputter, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CGoogleDriveReader>();

        /// <summary>
        /// Used to read downloaded file from Google Drive.
        /// </summary>
        private readonly CLocalFileReader _localFileReader;

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "GoogleDriveReader";

        #endregion


        /// <summary>
        /// Constructor which forwards drive service instance to base class.
        /// </summary>
        public CGoogleDriveReader(DriveService driveService, IFileReader fileReader)
            : base(driveService)
        {
            _localFileReader = new CLocalFileReader(fileReader);
        }

        /// <summary>
        /// Separate callback which is used in response processing when progress changed.
        /// </summary>
        /// <param name="progress">Reports download progress.</param>
        /// <param name="stream">Stream to save content from response.</param>
        /// <param name="saveTo">Filename to save on local storage.</param>
        /// <param name="mimeType">
        /// MIME type which is used to get correct extension for file.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <param name="progress">progress</param> has inconsistent value.
        /// </exception>
        private static void ProgressChanged_Callback(IDownloadProgress progress,
            MemoryStream stream, String saveTo, String mimeType)
        {
            switch (progress.Status)
            {
                case DownloadStatus.Downloading:
                    SGlobalMessageHandler.OutputMessage(
                        progress.BytesDownloaded.ToString()
                    );
                    break;

                case DownloadStatus.Completed:
                    s_logger.Info("Download completed.");
                    SGlobalMessageHandler.OutputMessage("Download completed.");
                    if (String.IsNullOrEmpty(mimeType))
                    {
                        SaveStream(stream, saveTo);
                    }
                    else
                    {
                        SaveStream(stream, saveTo + GetExtension(mimeType));
                    }
                    break;

                case DownloadStatus.Failed:
                    s_logger.Warn("Download failed.");
                    SGlobalMessageHandler.OutputMessage("Download failed.");
                    break;

                case DownloadStatus.NotStarted:
                    break;

                default:
                    var ex = new ArgumentOutOfRangeException(
                        nameof(progress), progress.Status, "Not caught switch statement!"
                    );
                    s_logger.Error(ex, $"Not caught value {progress.Status}!");
                    throw ex;
            }
        }

        #region IInputter Implementation

        /// <summary>
        /// Sends request to Google Drive API, downloads file and reads it.
        /// </summary>
        /// <param name="storageName">Storage name on Google Drive with Things names.</param>
        /// <returns>Processed collection of Things names as strings.</returns>
        public List<String> ReadThingNames(String storageName)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(storageName)) return result;

            // Get info from API, download file and read it.
            IList<GoogleDriveData.File> files = ListFiles(new CGoogleDriveFilesListOptionalParams
                { Q = $"name contains '{storageName}'" }).Files;

            if (!files.IsNullOrEmpty())
            {
                foreach (GoogleDriveData.File file in files)
                {
                    if (storageName.IsEqualWithInvariantCulture(file.Name))
                    {
                        result = DownloadAndReadFile(storageName, file.Id);
                        break;
                    }
                }
            }
            else
            {
                s_logger.Info($"No files found. Tried to find \"{storageName}\".");
                SGlobalMessageHandler.OutputMessage("No files found.");
            }

            return result;
        }

        #endregion

        /// <summary>
        /// Downloads file from Google Drive.
        /// </summary>
        /// <param name="fileId">File ID to download. You can get it from Google Drive API.</param>
        /// <param name="saveTo">Filename to save on local storage.</param>
        private void DownloadFile(String fileId, String saveTo)
        {
            using (var stream = new MemoryStream())
            {
                FilesResource.GetRequest request = GoogleDriveService.Files.Get(fileId);

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                    progress => ProgressChanged_Callback(progress, stream, saveTo, String.Empty);
                request.Download(stream);
            }
        }

        /// <inheritdoc cref="DownloadFile" />
        /// <param name="mimeType">
        /// MIME type which is used to get correct extension for file.
        /// </param>
        private void ExportFile(String fileId, String saveTo, String mimeType)
        {
            using (var stream = new MemoryStream())
            {
                FilesResource.ExportRequest request = GoogleDriveService.Files.Export(fileId,
                                                                                      mimeType);

                request.MediaDownloader.ProgressChanged +=
                    progress => ProgressChanged_Callback(progress, stream, saveTo, mimeType);
                request.Download(stream);
            }
        }

        /// <summary>
        /// Recognizes file extension and call appropriate downloading method. Then reads file with
        /// <see cref="CLocalFileReader" />.
        /// </summary>
        /// <param name="storageName">Storage name on the Google Drive with Things names.</param>
        /// <param name="fileId">File ID which is used to download file from Google Drive.</param>
        /// <returns>Collection of Things names as strings.</returns>
        /// <remarks>
        /// This method creates and deletes temporary file to store downloaded content.
        /// </remarks>
        private List<String> DownloadAndReadFile(String storageName, String fileId)
        {
            var result = new List<String>();
            try
            {
                if (HasExtenstionSafe(storageName))
                {
                    DownloadFile(fileId, storageName);
                }
                else
                {
                    const String mimeType = "text/csv";
                    ExportFile(fileId, storageName, mimeType);
                    storageName = storageName + GetExtension(mimeType);
                }

                result = _localFileReader.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                s_logger.Warn(ex, "An error occured during downloading and reading file.");
                SGlobalMessageHandler.OutputMessage("An error occured during downloading and " +
                                                    $"reading file: {ex.Message}");
            }
            finally
            {
                DeleteFile(storageName);
                s_logger.Debug($"Deleted temporary created file \"{storageName}\".");
            }
            return result;
        }
    }
}
