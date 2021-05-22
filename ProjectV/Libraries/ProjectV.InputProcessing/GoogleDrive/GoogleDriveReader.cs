using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Acolyte.Collections;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using ProjectV.IO.Input.File;
using ProjectV.Logging;
using GoogleDriveData = Google.Apis.Drive.v3.Data;

namespace ProjectV.IO.Input.GoogleDrive
{
    /// <summary>
    /// Concrete implementation of reader part for Google Drive API.
    /// </summary>
    public sealed class GoogleDriveReader : GoogleDriveWorker, IInputter, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<GoogleDriveReader>();

        /// <summary>
        /// Used to read downloaded file from Google Drive.
        /// </summary>
        private readonly LocalFileReader _localFileReader;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(GoogleDriveReader);

        #endregion


        /// <summary>
        /// Constructor which forwards drive service instance to base class.
        /// </summary>
        /// <param name="driveService">Google drive service instance.</param>
        /// <param name="fileReaderAsenc">Implementation to read files.</param>
        public GoogleDriveReader(DriveService driveService, IFileReader fileReaderAsenc)
            : base(driveService)
        {
            _localFileReader = new LocalFileReader(fileReaderAsenc);
        }

        #region IInputter Implementation

        /// <summary>
        /// Sends request to Google Drive API, downloads file and reads it.
        /// </summary>
        /// <param name="storageName">Storage name on Google Drive with Things names.</param>
        /// <returns>Enumeration of Things names as strings.</returns>
        public IEnumerable<string> ReadThingNames(string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return new List<string>();

            // Get info from API, download file and read it.
            // TODO: move query logic to separate class.
            IList<GoogleDriveData.File> files = ListFiles(new GoogleDriveFilesListOptionalParams
            { Q = $"name contains '{storageName}'" }).Files;

            if (!files.IsNullOrEmpty())
            {
                foreach (GoogleDriveData.File file in files)
                {
                    if (string.Equals(storageName, file.Name,
                                      StringComparison.InvariantCultureIgnoreCase))
                    {
                        return DownloadAndReadFile(storageName, file.Id);
                    }
                }
            }
            else
            {
                _logger.Info($"No files found. Tried to find \"{storageName}\".");
            }

            return Array.Empty<string>();
        }

        #endregion

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
            MemoryStream stream, string saveTo, string mimeType)
        {
            switch (progress.Status)
            {
                case DownloadStatus.Downloading:
                    {
                        _logger.Info($"Downloading: {progress.BytesDownloaded}");
                        break;
                    }

                case DownloadStatus.Completed:
                    {
                        _logger.Info("Download completed.");
                        if (string.IsNullOrEmpty(mimeType))
                        {
                            SaveStream(stream, saveTo);
                        }
                        else
                        {
                            SaveStream(stream, saveTo + GetExtension(mimeType));
                        }
                        break;
                    }

                case DownloadStatus.Failed:
                    {
                        _logger.Warn("Download failed.");
                        break;
                    }

                case DownloadStatus.NotStarted:
                    {
                        break;
                    }

                default:
                    {
                        throw new ArgumentOutOfRangeException(
                            nameof(progress), progress.Status,
                            $"Not known status value: '{progress.Status.ToString()}'."
                        );
                    }
            }
        }

        /// <summary>
        /// Downloads file from Google Drive.
        /// </summary>
        /// <param name="fileId">File ID to download. You can get it from Google Drive API.</param>
        /// <param name="saveTo">Filename to save on local storage.</param>
        private void DownloadFile(string fileId, string saveTo)
        {
            using var stream = new MemoryStream();

            FilesResource.GetRequest request = GoogleDriveService.Files.Get(fileId);

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged +=
                progress => ProgressChanged_Callback(progress, stream, saveTo, string.Empty);

            // TODO: we can use async overload but need to make changes in inputters interface.
            request.Download(stream);
        }

        /// <inheritdoc cref="DownloadFile" />
        /// <param name="mimeType">
        /// MIME type which is used to get correct extension for file.
        /// </param>
        private void ExportFile(string fileId, string saveTo, string mimeType)
        {
            using var stream = new MemoryStream();

            FilesResource.ExportRequest request = GoogleDriveService.Files.Export(fileId, mimeType);

            request.MediaDownloader.ProgressChanged +=
                progress => ProgressChanged_Callback(progress, stream, saveTo, mimeType);

            // TODO: we can use async overload but need to make changes in inputters interface.
            request.Download(stream);
        }

        /// <summary>
        /// Recognizes file extension and call appropriate downloading method. Then reads file with
        /// <see cref="LocalFileReader" />.
        /// </summary>
        /// <param name="storageName">Storage name on the Google Drive with Things names.</param>
        /// <param name="fileId">File ID which is used to download file from Google Drive.</param>
        /// <returns>Enumeration of Things names as strings.</returns>
        /// <remarks>
        /// This method creates and deletes temporary file to store downloaded content.
        /// </remarks>
        private IEnumerable<string> DownloadAndReadFile(string storageName, string fileId)
        {
            string? downloadedFilename = null;
            try
            {
                if (!TryDownloadFileSafe(storageName, fileId, out downloadedFilename))
                {
                    yield break;
                }

                foreach (string thingName in _localFileReader.ReadThingNames(downloadedFilename))
                {
                    yield return thingName;
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(downloadedFilename))
                {
                    DeleteFileSafe(downloadedFilename);
                }
            }
        }

        private bool TryDownloadFileSafe(string storageName, string fileId,
            [MaybeNullWhen(false)] out string downloadedFilename)
        {
            try
            {
                // Get temporary file name but remove extension because file reading logic depends
                // on extension.
                string tempFilename = Path.GetTempFileName();
                string tempFilepath = Path.Combine(
                    Path.GetDirectoryName(tempFilename),
                    Path.GetFileNameWithoutExtension(tempFilename)
                );

                if (HasExtenstionSafe(storageName))
                {
                    DownloadFile(fileId, tempFilepath);
                }
                else
                {
                    const string mimeType = "text/csv";
                    ExportFile(fileId, tempFilepath, mimeType);
                    tempFilepath += GetExtension(mimeType);
                }

                downloadedFilename = tempFilepath;
                return true;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "An error occurred during downloading and reading file.");

                downloadedFilename = null;
                return false;
            }
        }
    }
}
