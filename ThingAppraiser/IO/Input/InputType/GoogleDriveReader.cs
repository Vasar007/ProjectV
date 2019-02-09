using System;
using System.IO;
using System.Collections.Generic;
using Google.Apis.Download;

namespace ThingAppraiser.IO.Input
{
    public class GoogleDriveReader : GoogleDriveWorker, IInputter
    {
        private LocalFileReader _localFileReader = new LocalFileReader();

        private void DownloadFile(string fileId, string saveTo)
        {
            using (var stream = new MemoryStream())
            {
                var request = _driveService.Files.Get(fileId);

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
                            Console.WriteLine("Download completed.");
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
        }

        // Duplicated code because of there is not common interface for get and export requests.
        private void ExportFile(string fileId, string saveTo, string mimeType = "text/plain")
        {
            using (var stream = new MemoryStream())
            {
                var request = _driveService.Files.Export(fileId, mimeType);

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
                            SaveStream(stream, saveTo + GetExtension(mimeType));
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
        }

        private List<string> DownloadAndReadFile(string storageName, string fileId)
        {
            var result = new List<string>();
            try
            {
                if (HasExtention(storageName))
                {
                    DownloadFile(fileId, storageName);
                }
                else
                {
                    const string mimeType = "text/csv";
                    ExportFile(fileId, storageName, mimeType);
                    storageName = storageName + GetExtension(mimeType);
                }

                result = _localFileReader.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occured during downloading and reading " +
                                  $"file: {ex.Message}");
            }
            finally
            {
                DeleteDownloadedFile(storageName);
            }
            return result;
        }

        public List<string> ReadThingNames(string storageName)
        {
            // Get info from API, download file and read it.
            var files = ListFiles(new GoogleDriveFilesListOptionalParams()
                                  { Q = $"name contains '{storageName}'" }).Files;

            var result = new List<string>();
            if (!(files is null) && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (storageName == file.Name)
                    {
                        result = DownloadAndReadFile(storageName, file.Id);
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
