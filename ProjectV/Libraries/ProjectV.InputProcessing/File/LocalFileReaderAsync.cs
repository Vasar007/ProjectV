using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Logging;

namespace ProjectV.IO.Input.File
{
    /// <summary>
    /// Class which can read from local files.
    /// </summary>
    public sealed class LocalFileReaderAsync : IInputterAsync, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<LocalFileReaderAsync>();

        /// <summary>
        /// Helper variable to read data from file with additional processing.
        /// </summary>
        private readonly IFileReaderAsync _fileReaderAsync;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(LocalFileReaderAsync);

        #endregion


        /// <summary>
        /// Initializes instance with specified reader.
        /// </summary>
        /// <param name="fileReaderAsync">File reader implementation.</param>
        public LocalFileReaderAsync(
            IFileReaderAsync fileReaderAsync)
        {
            _fileReaderAsync = fileReaderAsync.ThrowIfNull(nameof(fileReaderAsync));
        }

        #region IInputterAsync Implementation

        /// <summary>
        /// Recognizes file extension and calls appropriate reading method.
        /// </summary>
        /// <param name="storageName">Storage with Things names.</param>
        /// <returns>Things names as collection of strings.</returns>
        public IEnumerable<string> ReadThingNames(string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return Enumerable.Empty<string>();

            try
            {
                if (storageName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
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
