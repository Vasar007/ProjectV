using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input.File
{
    public sealed class LocalFileReaderAsync : IInputterAsync, IInputterBase, ITagable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<LocalFileReaderAsync>();

        private readonly IFileReaderAsync _fileReaderAsync;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(LocalFileReaderAsync);

        #endregion


        public LocalFileReaderAsync(IFileReaderAsync fileReaderAsync)
        {
            _fileReaderAsync = fileReaderAsync.ThrowIfNull(nameof(fileReaderAsync));
        }

        #region IInputterAsync Implementation

        public async Task ReadThingNames(BufferBlock<string> queueToWrite, string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return;

            try
            {
                if (storageName.EndsWith(".csv"))
                {
                    await _fileReaderAsync.ReadCsvFile(queueToWrite, storageName);
                }
                else
                {
                    await _fileReaderAsync.ReadFile(queueToWrite, storageName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Async file reader throws exception.");
                throw;
            }

        }

        #endregion
    }
}
