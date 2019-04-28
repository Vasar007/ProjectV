using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Class which can read from files and parse it.
    /// </summary>
    public class CLocalFileReader : IInputter, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CLocalFileReader>();

        private readonly IFileReader _fileReader;

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "LocalFileReader";

        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CLocalFileReader(IFileReader fileReader)
        {
            _fileReader = fileReader.ThrowIfNull(nameof(fileReader));
        }

        #region IInputter Implementation

        /// <summary>
        /// Recognizes file extension and calls appropriate reading method.
        /// </summary>
        /// <param name="storageName">Storage with Things names.</param>
        /// <returns>Things names as collection of strings.</returns>
        public List<String> ReadThingNames(String storageName)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(storageName)) return result;

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
                s_logger.Error(ex, "File reader throws exception.");
                throw;
            }

            return result;
        }

        #endregion
    }
}
