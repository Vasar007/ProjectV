using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Class which can read from files and parse it. Uses FileHelpers library to delegate all
    /// routine work.
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
        public String Tag { get; } = "LocalFileReader";

        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CLocalFileReader(IFileReader fileReader)
        {
            _fileReader = fileReader.ThrowIfNull(nameof(fileReader));
        }

        /// <summary>
        /// Read local file without any restrictions.
        /// </summary>
        /// <param name="filename">Filename to read.</param>
        /// <returns>Row by row representation of file as collection.</returns>
        private static List<String> ReadRawFile(String filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<String>();
            using (var reader = new StreamReader(filename))
            {
                // Scan line and remove special symbols.
                String line = reader.ReadLine();
                while (!(line is null))
                {
                    result.Add(line.Trim('\r', '\n', ' '));
                    line = reader.ReadLine();
                }
            }
            return result.ToList();
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
