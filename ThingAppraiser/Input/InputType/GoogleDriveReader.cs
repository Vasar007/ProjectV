using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Download;

namespace ThingAppraiser.Input
{
    public class GoogleDriveReader : Inputter
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/drive-dotnet-quickstart.json
        private static readonly string[] _scopes = { DriveService.Scope.DriveReadonly };
        private static readonly string _applicationName = "ThingAppraiser";

        private DriveService _driveService;
        private LocalFileReader _localFileReader = new LocalFileReader();

        public GoogleDriveReader()
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
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            _driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName,
            });
        }

        /// <summary>
        /// Using reflection to apply optional parameters to the request.  
        /// </summary>
        /// <remarks>
        /// If the optonal parameters are null then we will just return the request as is.
        /// </remarks>
        /// <param name="request">The request.</param>
        /// <param name="optional">The optional parameters.</param>  
        private static object ApplyOptionalParms(object request, object optional)
        {
            if (optional is null) return request;

            var optionalProperties = (optional.GetType()).GetProperties();

            foreach (var property in optionalProperties)
            {
                // Copy value from optional parms to the request.
                // WARNING! They should have the same names and datatypes.
                var piShared = (request.GetType()).GetProperty(property.Name);
                if (property.GetValue(optional, null) != null)
                {
                    // TODO: test that we do not add values for items that are null.
                    piShared?.SetValue(request, property.GetValue(optional, null), null);
                }
            }
            return request;
        }

        /// <summary>
        /// Lists or searches files.
        /// </summary>
        /// <see cref="https://developers.google.com/drive/v3/reference/files/list"/>
        /// <remarks>
        /// Generation Note: This does not always build corectly.  Google needs to standardise things I need to figuer out which ones are wrong.
        /// </remarks>
        /// <param name="optional">Optional paramaters.</param>
        private Google.Apis.Drive.v3.Data.FileList ListFiles(
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
                throw new Exception("Request Files.List failed.", ex);
            }
        }

        private void SaveStream(MemoryStream stream, string saveTo)
        {
            using (var file = new FileStream(saveTo, FileMode.Create, FileAccess.Write))
            {
                stream.WriteTo(file);
            }
        }

        private void DownloadFile(string fileId, string saveTo)
        {
            var request = _driveService.Files.Get(fileId);
            var stream = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                    {
                        Console.WriteLine(progress.BytesDownloaded);
                        break;
                    }
                    case DownloadStatus.Completed:
                    {
                        Console.WriteLine("Download complete.");
                        SaveStream(stream, saveTo);
                        break;
                    }
                    case DownloadStatus.Failed:
                    {
                        Console.WriteLine("Download failed.");
                        break;
                    }
                }
            };
            request.Download(stream);
        }

        private string AddExtension(string mimeType)
        {
            switch (mimeType)
            {
                case "text/plain": return ".txt";
                case "text/csv": return ".csv";
                case "text/html": return ".html";
                case "application/rtf": return ".rtf";
                case "application/pdf": return ".pdf";
                default:
                    Console.WriteLine($"Not found such MIME type: {mimeType}");
                    return "";
            }
        }

        // Duplicated code because of there is not common interface for requests.
        private void ExportFile(string fileId, string saveTo, string mimeType = "text/plain")
        {
            var request = _driveService.Files.Export(fileId, mimeType);
            var stream = new MemoryStream();

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged += (IDownloadProgress progress) =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                    {
                        Console.WriteLine(progress.BytesDownloaded);
                        break;
                    }
                    case DownloadStatus.Completed:
                    {
                        Console.WriteLine("Download complete.");
                        SaveStream(stream, saveTo + AddExtension(mimeType));
                        break;
                    }
                    case DownloadStatus.Failed:
                    {
                        Console.WriteLine("Download failed.");
                        break;
                    }
                }
            };
            request.Download(stream);
        }

        private IList<Google.Apis.Drive.v3.Data.File> GetFiles(int pageSize = 10,
            string fields = "nextPageToken, files(id, name)")
        {
            // Define parameters of request.
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = pageSize;
            listRequest.Fields = fields;

            // List files.
            return listRequest.Execute().Files;
        }

        private bool DeleteDownloadedFile(string filename)
        {
            try
            {
                File.Delete(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't delete donwloaded file {filename}. " +
                                  $"Error:{ex.Message}");
                return false;
            }
            return true;
        }

        private bool HasExtention(string filename)
        {
            return filename.Contains(".");
        }

        public override List<string> ReadNames(string storageName)
        {
            var files = ListFiles(new GoogleDriveFilesListOptionalParams()
                                  { Q = $"name contains '{storageName}'" }).Files;

            var result = new List<string>();

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (storageName == file.Name)
                    {
                        const string mimeType = "text/csv";
                        if (HasExtention(storageName)) DownloadFile(file.Id, storageName);
                        else ExportFile(file.Id, storageName, mimeType);

                        storageName = storageName + AddExtension(mimeType);
                        result = _localFileReader.ReadNames(storageName);
                        DeleteDownloadedFile(storageName);
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("No files found.");
            }
            return result;
        }
    }
}
