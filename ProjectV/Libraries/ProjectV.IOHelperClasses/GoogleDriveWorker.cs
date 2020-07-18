using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Acolyte.Assertions;
using Google.Apis.Drive.v3;
using GoogleDriveData = Google.Apis.Drive.v3.Data;
using ProjectV.Communication;
using ProjectV.Logging;

namespace ProjectV.IO
{
    /// <summary>
    /// Base class to interact with Google Drive API.
    /// </summary>
    public abstract class GoogleDriveWorker
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<GoogleDriveWorker>();

        /// <summary>
        /// Available OAuth scopes to use with the Drive API.
        /// </summary>
        /// <remarks>
        /// If modifying these scopes, delete your previously saved credentials
        /// at ~/.credentials/tokens.json
        /// </remarks>
        public static IReadOnlyList<string> Scopes { get; } = new[] { DriveService.Scope.Drive };

        /// <summary>
        /// The name of the application that is used to obtain the required credentials.
        /// </summary>
        public static string ApplicationName { get; } = "ProjectV";

        /// <summary>
        /// Instance of Drive Service to send requests and get responses from Google Drive API.
        /// </summary>
        protected DriveService GoogleDriveService { get; }


        /// <summary>
        /// Initializes worker with specified service to interact with GoogleDrive API.
        /// </summary>
        /// <param name="driveService">
        /// Instance of Drive Service. Use service builder class to get it right.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="driveService" /> is <c>null</c>.
        /// </exception>
        protected GoogleDriveWorker(DriveService driveService)
        {
            GoogleDriveService = driveService.ThrowIfNull(nameof(driveService));
        }

        /// <summary>
        /// Using reflection to apply optional parameters to the request.  
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="optional">The optional parameters.</param>  
        /// <remarks>
        /// If the optional parameters are <c>null</c> then we will just return the request as is.
        /// </remarks>
        protected static object ApplyOptionalParams(object request, object optional)
        {
            if (optional is null) return request;

            PropertyInfo[] optionalProperties = optional.GetType().GetProperties();
            foreach (PropertyInfo property in optionalProperties)
            {
                // Copy value from optional parms to the request.
                // WARNING! They should have the same names and data types.
                PropertyInfo piShared = request.GetType().GetProperty(property.Name);
                object propertyValue = property.GetValue(optional, null);

                // Test that we do not add values for items that are null.
                if (!(propertyValue is null) && !(piShared is null))
                {
                    piShared.SetValue(request, propertyValue, null);
                }
            }
            return request;
        }

        /// <summary>
        /// Saves stream content to file.
        /// </summary>
        /// <param name="stream">Stream to process.</param>
        /// <param name="saveTo">Filename (absolute or relative).</param>
        protected static void SaveStream(MemoryStream stream, string saveTo)
        {
            using var file = new FileStream(saveTo, FileMode.Create, FileAccess.Write);

            stream.WriteTo(file);
        }

        /// <summary>
        /// Gets extension from MIME type.
        /// </summary>
        /// <param name="mimeType">MIME type to process.</param>
        /// <returns>Appropriate extension to passed type.</returns>
        protected static string GetExtension(string mimeType)
        {
            return mimeType switch
            {
                "text/plain" => ".txt",

                "text/csv" => ".csv",

                "text/html" => ".html",

                "application/rtf" => ".rtf",

                "application/pdf" => ".pdf",

                _ => throw new ArgumentOutOfRangeException(
                         nameof(mimeType), mimeType,
                         $"Not found extension for MIME type: '{mimeType}'."
                     )
            };
        }

        /// <summary>
        /// Gets MIME type from filename.
        /// </summary>
        /// <param name="filename">Filename to process.</param>
        /// <returns>Appropriate MIME type to passed file extension.</returns>
        /// <exception cref="ArgumentException">
        /// <param name="filename">filename</param> isn't contain extension.
        /// </exception>
        protected static string GetMimeType(string filename)
        {
            if (!HasExtenstionSafe(filename))
            {
                throw new ArgumentException($"Filename '{filename}' isn't contain extension.",
                                            nameof(filename));
            }

            string extension = Path.GetExtension(filename);
            return extension switch
            {
                ".txt" => "text/plain",

                ".csv" => "text/csv",

                ".html" => "text/html",

                ".rtf" => "application/rtf",

                ".pdf" => "application/pdf",

                _ => throw new ArgumentOutOfRangeException(
                         nameof(filename), filename,
                         $"Not found MIME type for extension \"{extension}\" for " +
                         $"file \"{filename}\"."
                     )
            };
        }

        /// <summary>
        /// Deletes the specified file.
        /// </summary>
        /// <param name="filename">Filename to delete.</param>
        /// <returns><c>true</c> if no exception occured, <c>false</c> otherwise.</returns>
        protected static bool DeleteFileSafe(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Could not delete downloaded file \"{filename}\".");
                GlobalMessageHandler.OutputMessage("Could not delete downloaded file " +
                                                    $" \"{filename}\". Error: {ex}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if filename has extension or not.
        /// </summary>
        /// <param name="filename">Filename to process.</param>
        /// <returns>
        /// <c>true</c> if filename has extension, <c>false</c> otherwise (including case when
        /// <param name="filename">filename</param> is <c>null</c> or presents empty string).
        /// </returns>
        protected static bool HasExtenstionSafe(string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            return filename.Contains(".");
        }

        /// <summary>
        /// Lists or searches files.
        /// </summary>
        /// <param name="optional">Optional parameters.</param>
        /// <returns>Response from Google Drive API as list files.</returns>
        /// <remarks>
        /// Generation Note: This does not always build correctly. Google needs to standardize
        /// things because breaking changes may cause errors.
        /// </remarks>
        /// <seealso href="https://developers.google.com/drive/v3/reference/files/list" />
        /// <exception cref="InvalidOperationException">Request failed.</exception>
        protected GoogleDriveData.FileList ListFiles(
            GoogleDriveFilesListOptionalParams optional)
        {
            try
            {
                // Building the initial request.
                FilesResource.ListRequest request = GoogleDriveService.Files.List();
                // Applying optional parameters to the request.                
                request = (FilesResource.ListRequest) ApplyOptionalParams(request, optional);
                // Requesting data.
                return request.Execute();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Request Files.List failed.");
                throw new InvalidOperationException("Request Files.List failed.", ex);
            }
        }

        /// <summary>
        /// Executes list requests and gets list files.
        /// </summary>
        /// <param name="pageSize">The maximum number of files return per page.</param>
        /// <param name="fields">Selector specifying which fields to include in a response.</param>
        /// <returns>The list of files from request.</returns>
        protected IList<GoogleDriveData.File> GetFiles(int pageSize, string fields)
        {
            // Define parameters of request.
            FilesResource.ListRequest listRequest = GoogleDriveService.Files.List();
            listRequest.PageSize = pageSize;
            listRequest.Fields = fields;

            // List files.
            return listRequest.Execute().Files;
        }

        /// <summary>
        /// Executes list requests and gets list files. Gets 10 search results and defines selector.
        /// </summary>
        /// <returns>The list of files from request.</returns>
        protected IList<GoogleDriveData.File> GetFiles()
        {
            // Call methods with default parameters.
            return GetFiles(10, "nextPageToken, files(id, name)");
        }
    }
}
