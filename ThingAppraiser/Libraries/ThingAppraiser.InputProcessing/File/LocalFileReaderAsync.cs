using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Extensions;
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

        public IEnumerable<string> ReadThingNames(string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return Enumerable.Empty<string>();

            try
            {
                if (storageName.EndsWith(".csv"))
                {
                    return _fileReaderAsync.ReadCsvFile(storageName);
                }
                else
                {
                    return _fileReaderAsync.ReadFile(storageName);
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
