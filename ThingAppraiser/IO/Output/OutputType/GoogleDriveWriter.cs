using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Upload;
using GoogleDriveData = Google.Apis.Drive.v3.Data;

namespace ThingAppraiser.IO.Output
{
    public class GoogleDriveWriter : GoogleDriveWorker, IOutputter
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // The default chunk size is 10MB for Google Drive API uploading methods.
        private const int _maxFileLength = 10_000_000;

        private LocalFileWriter _localFileWriter = new LocalFileWriter();

        private bool UploadFile(string filename,
            string mimeTypeToUpload = "application/vnd.google-apps.spreadsheet")
        {
            using (var stream = new FileStream(filename, FileMode.Open))
            {
                var request = _driveService.Files.Create(
                    new GoogleDriveData.File
                    {
                        Name = filename,
                        MimeType = mimeTypeToUpload ?? string.Empty
                    },
                    stream,
                    GetMimeType(filename)
                );

                // Add handlers which will be notified on progress changes and upload completion.
                // Notification of progress changed will be invoked when the upload was started,
                // on each upload chunk, and on success or failure.
                request.ProgressChanged += Upload_ProgressChanged;
                request.ResponseReceived += Upload_ResponseReceived;

                var result = request.Upload();
                return result.Status == UploadStatus.Completed;
            }
        }

        /// <summary>
        /// Update an existing file's metadata and content.
        /// </summary>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="filename">Filename of the new content to upload.</param>
        /// <param name="newName">New title for the file.</param>
        /// <param name="newMimeType">New MIME type for the file.</param>
        /// <returns>Updated file metadata, null is returned if an API error occurred.</returns>
        private bool UpdateFile(string fileId, string filename,
            string newName = null, string newMimeType = null)
        {
            // First retrieve the file from the API.
            var file = _driveService.Files.Get(fileId).Execute();

            // File's new metadata.
            var newContent = new GoogleDriveData.File
            {
                MimeType = newMimeType ?? file.MimeType,
                Name = newName ?? file.Name
            };

            using (var stream = new FileStream(filename, FileMode.Open))
            {
                
                if (stream.Length > _maxFileLength)
                {
                    _logger.Error($"File size {stream.Length} is greater than " +
                                   "maximum for uploading (10MB).");
                    throw new ArgumentException($"File size {stream.Length} is greater than " +
                                                "maximum for uploading (10MB).");
                }

                // Send the request to the API.
                var request = _driveService.Files.Update(newContent, fileId, stream,
                                                         newContent.MimeType);

                // Add handlers which will be notified on progress changes and upload completion.
                // Notification of progress changed will be invoked when the upload was started,
                // on each upload chunk, and on success or failure.
                request.ProgressChanged += Upload_ProgressChanged;
                request.ResponseReceived += Upload_ResponseReceived;

                var result = request.Upload();
                return result.Status == UploadStatus.Completed;
            }
        }

        private bool ReadFileAndUploadOrUpdate(string storageName)
        {
            var storageNameWithoutExtension = Path.GetFileNameWithoutExtension(storageName);
            var files = ListFiles(new GoogleDriveFilesListOptionalParams()
            { Q = $"name contains '{storageNameWithoutExtension}'" }).Files;

            bool result = false;
            if (!(files is null) && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (storageNameWithoutExtension == file.Name)
                    {
                        // Need full filename because we will read file and upload it.
                        result = UpdateFile(file.Id, storageName);
                        break;
                    }
                }
            }
            else
            {
                result = UploadFile(storageName);
            }
            return result;
        }

        private static void Upload_ProgressChanged(IUploadProgress progress)
        {
            _logger.Info($"{progress.Status} {progress.BytesSent}");
            Core.Shell.OutputMessage($"{progress.Status} {progress.BytesSent}");
        }

        private static void Upload_ResponseReceived(GoogleDriveData.File file)
        {
            _logger.Info($"\"{file.Name}\" was uploaded successfully.");
            Core.Shell.OutputMessage($"\"{file.Name}\" was uploaded successfully.");
        }

        public bool SaveResults(List<List<Data.ResultType>> results, string storageName)
        {
            // Save results to local file and upload it.
            if (!_localFileWriter.SaveResults(results, storageName)) return false;

            try
            {
                return ReadFileAndUploadOrUpdate(storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"An error occured during uploading to \"{storageName}\".");
                Core.Shell.OutputMessage($"An error occured during uploading to \"{storageName}\"" +
                                         $": {ex.Message}");
                return false;
            }
            finally
            {
                DeleteFile(storageName);
                _logger.Debug($"Deleted temporary created file \"{storageName}\".");
            }
        }
    }
}
