using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Upload;
using Google.Apis.Drive.v3;
using GoogleDriveData = Google.Apis.Drive.v3.Data;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;
using ThingAppraiser.IO.Output.File;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output.GoogleDrive
{
    /// <summary>
    /// Concrete implementation of writer part for Google Drive API.
    /// </summary>
    public sealed class GoogleDriveWriter : GoogleDriveWorker, IOutputter, IOutputterBase, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<GoogleDriveWriter>();

        /// <summary>
        /// The default chunk size is 10MB for Google Drive API uploading methods.
        /// </summary>
        private static readonly int _maxFileLength = 10_000_000;

        /// <summary>
        /// Used to write results to local file which would be upload to Google Drive.
        /// </summary>
        private readonly LocalFileWriter _localFileWriter = new LocalFileWriter();

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(GoogleDriveWriter);

        #endregion


        /// <summary>
        /// Constructor which forwards drive service instance to base class.
        /// </summary>
        public GoogleDriveWriter(DriveService driveService)
            : base(driveService)
        {
        }

        #region IOutputter Implementation

        /// <summary>
        /// Saves results to Google Drive file.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="storageName">Storage name to write on Google Drive.</param>
        /// <returns>True if saving was successful, false otherwise.</returns>
        /// <remarks>
        /// This method creates and deletes temporary file to store appraised content.
        /// </remarks>
        public bool SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return false;

            string tempStorageName = "temp_" + storageName;
            while (LocalFileWriter.DoesExistFile(tempStorageName))
            {
                tempStorageName = "temp_" + tempStorageName;
            }

            // Save results to local file and upload it.
            if (!_localFileWriter.SaveResults(results, tempStorageName))
            {
                return false;
            }

            try
            {
                return ReadFileAndUploadOrUpdate(tempStorageName, storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"An error occured during uploading \"{storageName}\".");
                GlobalMessageHandler.OutputMessage("An error occured during uploading " +
                                                   $"\"{storageName}\" : {ex}");
                return false;
            }
            finally
            {
                DeleteFileSafe(tempStorageName);
                _logger.Debug($"Deleted temporary created file \"{tempStorageName}\".");
            }
        }

        #endregion

        /// <summary>
        /// Separate callback which is used in response processing when progress changed.
        /// </summary>
        /// <param name="progress">Reporting upload progress.</param>
        private static void Upload_ProgressChanged(IUploadProgress progress)
        {
            _logger.Info($"{progress.Status} {progress.BytesSent}");
            GlobalMessageHandler.OutputMessage($"{progress.Status} {progress.BytesSent}");
        }

        /// <summary>
        /// Separate callback which is used in response processing when response received.
        /// </summary>
        /// <param name="file">The meta data for file.</param>
        private static void Upload_ResponseReceived(GoogleDriveData.File file)
        {
            _logger.Info($"\"{file.Name}\" was uploaded successfully.");
            GlobalMessageHandler.OutputMessage($"\"{file.Name}\" was uploaded successfully.");
        }

        /// <summary>
        /// Uploads file to the Google Drive, overrides file if such exists.
        /// </summary>
        /// <param name="path">Local file path to upload.</param>
        /// <param name="name">Filename to set.</param>
        /// <param name="mimeTypeToUpload">
        /// MIME type which is used to specify output format.
        /// </param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise</returns>
        private bool UploadFile(string path, string name, string mimeTypeToUpload)
        {
            using var stream = new FileStream(path, FileMode.Open);

            FilesResource.CreateMediaUpload request = GoogleDriveService.Files.Create(
                new GoogleDriveData.File
                {
                    Name = name,
                    MimeType = mimeTypeToUpload ?? string.Empty
                },
                stream,
                GetMimeType(path)
            );

            // Add handlers which will be notified on progress changes and upload completion.
            // Notification of progress changed will be invoked when the upload was started,
            // on each upload chunk, and on success or failure.
            request.ProgressChanged += Upload_ProgressChanged;
            request.ResponseReceived += Upload_ResponseReceived;

            IUploadProgress result = request.Upload();
            return result.Status == UploadStatus.Completed;
        }

        /// <summary>
        /// Uploads file to the Google Drive, overrides file if such exists. Specifies Google Sheets
        /// output format.
        /// </summary>
        /// <param name="path">Local file path to upload.</param>
        /// <param name="name">Filename to set.</param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise</returns>
        private bool UploadFile(string path, string name)
        {
            return UploadFile(path, name, "application/vnd.google-apps.spreadsheet");
        }

        /// <summary>
        /// Updates an existing file's metadata and content.
        /// </summary>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="path">Filename of the new content to upload.</param>
        /// <param name="newName">New title for the file.</param>
        /// <param name="newMimeType">New MIME type for the file.</param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Maximum uploading file size exceeded.
        /// </exception>
        private bool UpdateFile(string fileId, string path, string? newName, string? newMimeType)
        {
            // First retrieve the file from the API.
            GoogleDriveData.File file = GoogleDriveService.Files.Get(fileId).Execute();

            // File's new metadata.
            var newContent = new GoogleDriveData.File
            {
                MimeType = newMimeType ?? file.MimeType,
                Name = newName ?? file.Name
            };

            using var stream = new FileStream(path, FileMode.Open);

            if (stream.Length > _maxFileLength)
            {
                throw new ArgumentOutOfRangeException(nameof(stream), stream.Length,
                    $"File size {stream.Length} is greater than maximum for uploading (10MB)."
                );
            }

            // Send the request to the API.
            FilesResource.UpdateMediaUpload request = GoogleDriveService.Files.Update(
                newContent, fileId, stream, newContent.MimeType
            );

            // Add handlers which will be notified on progress changes and upload completion.
            // Notification of progress changed will be invoked when the upload was started,
            // on each upload chunk, and on success or failure.
            request.ProgressChanged += Upload_ProgressChanged;
            request.ResponseReceived += Upload_ResponseReceived;

            IUploadProgress result = request.Upload();
            return result.Status == UploadStatus.Completed;
        }

        /// <summary>
        /// Updates an existing file's metadata and content. Doesn't change neither filename nor 
        /// mime type.
        /// </summary>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="path">Local file path to upload.</param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise.</returns>
        private bool UpdateFile(string fileId, string path)
        {
            return UpdateFile(fileId, path, null, null);
        }

        /// <summary>
        /// Checks if file with specified name exists on Google Drive and in this regard decides to
        /// upload or update results which saved in local file.
        /// </summary>
        /// <param name="path">Local file path to upload.</param>
        /// <param name="storageName">Local filename to save on Google Drive.</param>
        /// <returns><c>true</c> if procedure was successful, <c>false</c> otherwise.</returns>
        private bool ReadFileAndUploadOrUpdate(string path, string storageName)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(storageName)) return false;

            string storageNameWithoutExtension = Path.GetFileNameWithoutExtension(storageName);
            IList<GoogleDriveData.File> files = ListFiles(new GoogleDriveFilesListOptionalParams
                { Q = $"name contains '{storageNameWithoutExtension}'" }).Files;

            bool result = false;
            if (!files.IsNullOrEmpty())
            {
                foreach (GoogleDriveData.File file in files)
                {
                    if (storageNameWithoutExtension == file.Name)
                    {
                        // Need full filename because we will read file and upload it.
                        result = UpdateFile(file.Id, path);
                        break;
                    }
                }
            }
            else
            {
                result = UploadFile(path, storageName);
            }

            return result;
        }
    }
}
