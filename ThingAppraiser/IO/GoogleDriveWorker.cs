using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace ThingAppraiser.IO
{
    public abstract class GoogleDriveWorker
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/tokens.json
        private static readonly string[] _scopes = { DriveService.Scope.Drive };
        private const string _applicationName = "ThingAppraiser";

        protected static DriveService _driveService;

        static GoogleDriveWorker()
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    _applicationName,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                _logger.Info($"Credential file saved to: {credPath}");
                Core.Shell.OutputMessage($"Credential file saved to: {credPath}");
            }

            // Create Drive API service.
            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });
        }

        /// <summary>
        /// Lists or searches files.
        /// </summary>
        /// <see cref="https://developers.google.com/drive/v3/reference/files/list"/>
        /// <remarks>
        /// Generation Note: This does not always build corectly.  Google needs to standardise things I need to figuer out which ones are wrong.
        /// </remarks>
        /// <param name="optional">Optional paramaters.</param>
        protected Google.Apis.Drive.v3.Data.FileList ListFiles(
            GoogleDriveFilesListOptionalParams optional = null)
        {
            try
            {
                // Building the initial request.
                var request = _driveService.Files.List();
                // Applying optional parameters to the request.                
                request = ApplyOptionalParms(request, optional) as FilesResource.ListRequest;
                // Requesting data.
                return request.Execute();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Request Files.List failed.");
                throw new Exception("Request Files.List failed.", ex);
            }
        }

        protected IList<Google.Apis.Drive.v3.Data.File> GetFiles(int pageSize = 10,
            string fields = "nextPageToken, files(id, name)")
        {
            // Define parameters of request.
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = pageSize;
            listRequest.Fields = fields;

            // List files.
            return listRequest.Execute().Files;
        }

        /// <summary>
        /// Using reflection to apply optional parameters to the request.  
        /// </summary>
        /// <remarks>
        /// If the optonal parameters are null then we will just return the request as is.
        /// </remarks>
        /// <param name="request">The request.</param>
        /// <param name="optional">The optional parameters.</param>  
        protected static object ApplyOptionalParms(object request, object optional)
        {
            if (optional is null)
                return request;

            var optionalProperties = optional.GetType().GetProperties();

            foreach (var property in optionalProperties)
            {
                // Copy value from optional parms to the request.
                // WARNING! They should have the same names and datatypes.
                var piShared = (request.GetType()).GetProperty(property.Name);
                var propertyValue = property.GetValue(optional, null);

                // Test that we do not add values for items that are null.
                if (!(propertyValue is null) && !(piShared is null))
                {
                    piShared.SetValue(request, propertyValue, null);
                }
            }
            return request;
        }

        protected static void SaveStream(MemoryStream stream, string saveTo)
        {
            using (var file = new FileStream(saveTo, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        protected static string GetExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "text/plain":
                    return ".txt";
                case "text/csv":
                    return ".csv";
                case "text/html":
                    return ".html";
                case "application/rtf":
                    return ".rtf";
                case "application/pdf":
                    return ".pdf";
                default:
                    _logger.Warn($"Not found extension for MIME type: {mimeType}");
                    Core.Shell.OutputMessage($"Not found extension for MIME type: {mimeType}");
                    return default(string);
            }
        }

        protected static string GetMimeType(string filename)
        {
            if (!HasExtention(filename))
            {
                _logger.Error($"Filename {filename} isn't contain extension.");
                throw new ArgumentException($"Filename {filename} isn't contain extension.");
            }

            switch (Path.GetExtension(filename))
            {
                case ".txt":
                    return "text/plain";
                case ".csv":
                    return "text/csv";
                case ".html":
                    return "text/html";
                case ".rtf":
                    return "application/rtf";
                case ".pdf":
                    return "application/pdf";
                default:
                    _logger.Warn($"Not found MIME type for extension: {filename}");
                    Core.Shell.OutputMessage($"Not found MIME type for extension: {filename}");
                    return default(string);
            }
        }

        protected static bool DeleteFile(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Couldn't delete donwloaded file {filename}.");
                Core.Shell.OutputMessage($"Couldn't delete donwloaded file {filename}. " +
                                          $"Error: {ex.Message}");
                return false;
            }
            return true;
        }

        protected static bool HasExtention(string filename)
        {
            return filename.Contains(".");
        }
    }
}
