using System;
using System.Collections.Generic;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input.File
{
    /// <summary>
    /// Class which can read from files and parse it.
    /// </summary>
    public sealed class LocalFileReader : IInputter, IInputterBase, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<LocalFileReader>();

        /// <summary>
        /// Helper variable to read data from file with additional processing.
        /// </summary>
        private readonly IFileReader _fileReader;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(LocalFileReader);

        #endregion


        /// <summary>
        /// Initializes instance with specified reader.
        /// </summary>
        public LocalFileReader(IFileReader fileReader)
        {
            _fileReader = fileReader.ThrowIfNull(nameof(fileReader));
        }

        #region IInputter Implementation

        /// <summary>
        /// Recognizes file extension and calls appropriate reading method.
        /// </summary>
        /// <param name="storageName">Storage with Things names.</param>
        /// <returns>Things names as collection of strings.</returns>
        public IReadOnlyList<string> ReadThingNames(string storageName)
        {
            var result = new List<string>();
            if (string.IsNullOrEmpty(storageName)) return result;

            try
            {
                if (storageName.EndsWith(".csv"))
                {
                    result = _fileReader.ReadCsvFile(storageName);
                }
                else
                {
                    result = _fileReader.ReadFile(storageName);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "File reader throws exception.");
                throw;
            }

            return result;
        }

        #endregion
    }
}
