using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Apis.Upload;
using Google.Apis.Drive.v3;
using GoogleDriveData = Google.Apis.Drive.v3.Data;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Concrete implementation of writer part for Google Drive API.
    /// </summary>
    public class CGoogleDriveWriter : CGoogleDriveWorker, IOutputter, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CGoogleDriveWriter>();

        /// <summary>
        /// The default chunk size is 10MB for Google Drive API uploading methods.
        /// </summary>
        private static readonly Int32 s_maxFileLength = 10_000_000;

        /// <summary>
        /// Used to write results to local file which would be upload to Google Drive.
        /// </summary>
        private readonly CLocalFileWriter _localFileWriter = new CLocalFileWriter();

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag { get; } = "GoogleDriveWriter";

        #endregion


        /// <summary>
        /// Constructor which forwards drive service instance to base class.
        /// </summary>
        public CGoogleDriveWriter(DriveService driveService)
            : base(driveService)
        {
        }

        /// <summary>
        /// Separate callback which is used in response processing when progress changed.
        /// </summary>
        /// <param name="progress">Reporting upload progress.</param>
        private static void Upload_ProgressChanged(IUploadProgress progress)
        {
            s_logger.Info($"{progress.Status} {progress.BytesSent}");
            SGlobalMessageHandler.OutputMessage($"{progress.Status} {progress.BytesSent}");
        }

        /// <summary>
        /// Separate callback which is used in response processing when response received.
        /// </summary>
        /// <param name="file">The meta data for file.</param>
        private static void Upload_ResponseReceived(GoogleDriveData.File file)
        {
            s_logger.Info($"\"{file.Name}\" was uploaded successfully.");
            SGlobalMessageHandler.OutputMessage($"\"{file.Name}\" was uploaded successfully.");
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
        public Boolean SaveResults(List<CRating> results, String storageName)
        {
            if (String.IsNullOrEmpty(storageName)) return false;

            String tempStorageName = "temp_" + storageName;
            while (CLocalFileWriter.DoesExistFile(tempStorageName))
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
                s_logger.Warn(ex, $"An error occured during uploading \"{storageName}\".");
                SGlobalMessageHandler.OutputMessage("An error occured during uploading " +
                                                    $"\"{storageName}\" : {ex.Message}");
                return false;
            }
            finally
            {
                DeleteFile(tempStorageName);
                s_logger.Debug($"Deleted temporary created file \"{tempStorageName}\".");
            }
        }

        #endregion

        /// <summary>
        /// Uploads file to the Google Drive, overrides file if such exists.
        /// </summary>
        /// <param name="path">Local file path to upload.</param>
        /// <param name="name">Filename to set.</param>
        /// <param name="mimeTypeToUpload">
        /// MIME type which is used to specify output format.
        /// </param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise</returns>
        private Boolean UploadFile(String path, String name, String mimeTypeToUpload)
        {
            using (var stream = new FileStream(path, FileMode.Open))
            {
                FilesResource.CreateMediaUpload request = GoogleDriveService.Files.Create(
                    new GoogleDriveData.File
                    {
                        Name = name,
                        MimeType = mimeTypeToUpload ?? String.Empty
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
        }

        /// <summary>
        /// Uploads file to the Google Drive, overrides file if such exists. Specifies Google Sheets
        /// output format.
        /// </summary>
        /// <param name="path">Local file path to upload.</param>
        /// <param name="name">Filename to set.</param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise</returns>
        private Boolean UploadFile(String path, String name)
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
        private Boolean UpdateFile(String fileId, String path, String newName, String newMimeType)
        {
            // First retrieve the file from the API.
            GoogleDriveData.File file = GoogleDriveService.Files.Get(fileId).Execute();

            // File's new metadata.
            var newContent = new GoogleDriveData.File
            {
                MimeType = newMimeType ?? file.MimeType,
                Name = newName ?? file.Name
            };

            using (var stream = new FileStream(path, FileMode.Open))
            {
                if (stream.Length > s_maxFileLength)
                {
                    var ex = new ArgumentOutOfRangeException(nameof(stream), stream.Length,
                        $"File size {stream.Length} is greater than maximum for uploading (10MB)."
                    );
                    s_logger.Error(ex, "Maximum uploading file size exceeded.");
                    throw ex;
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
        }

        /// <summary>
        /// Updates an existing file's metadata and content. Doesn't change neither filename nor 
        /// mime type.
        /// </summary>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="path">Local file path to upload.</param>
        /// <returns><c>true</c> if uploading was successful, <c>false</c> otherwise.</returns>
        private Boolean UpdateFile(String fileId, String path)
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
        private Boolean ReadFileAndUploadOrUpdate(String path, String storageName)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(storageName)) return false;

            String storageNameWithoutExtension = Path.GetFileNameWithoutExtension(storageName);
            IList<GoogleDriveData.File> files = ListFiles(new CGoogleDriveFilesListOptionalParams
                { Q = $"name contains '{storageNameWithoutExtension}'" }).Files;

            Boolean result = false;
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
