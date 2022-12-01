using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using FileHelpers;
using ProjectV.Logging;

namespace ProjectV.IO.Input.File
{
    /// <summary>
    /// Provides reading data from files with some filtering. Now this class filters by value of
    /// status field.
    /// </summary>
    public sealed class FilterFileReader : IFileReader
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<FilterFileReader>();

        /// <summary>
        /// Name of the column with status which can contain some details about Thing.
        /// </summary>
        private readonly string _statusHeader = "Status";

        /// <summary>
        /// Name of the column with Thing name.
        /// </summary>
        private readonly string _thingNameHeader = "Thing Name";


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public FilterFileReader()
        {
        }

        #region IFileReader Implementation

        /// <inheritdoc />
        /// <remarks>
        /// File must contain specified columns: <see cref="_thingNameHeader" />,
        /// <see cref="_statusHeader" />.
        /// </remarks>
        public IEnumerable<string> ReadFile(string filename)
        {
            _logger.Info($"Reading file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            using var engine = new FileHelperAsyncEngine<FilterInputFileData>();
            using (engine.BeginReadFile(filename))
            {
                foreach (FilterInputFileData record in engine)
                {
                    if (!string.IsNullOrEmpty(record.status)) continue;

                    if (result.Add(record.thingName))
                    {
                        yield return record.thingName;
                    }
                }
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// File must contain specified columns: <see cref="_thingNameHeader" />,
        /// <see cref="_statusHeader" />.
        /// </remarks>
        /// <exception cref="InvalidDataException">CSV file doesn't contain header.</exception>
        public IEnumerable<string> ReadCsvFile(string filename)
        {
            _logger.Info($"Reading CSV file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            var csvConfig = new CsvConfiguration(CultureInfo.CurrentCulture)
            {
                HasHeaderRecord = true
            };

            using var reader = new StreamReader(filename);
            using var csv = new CsvReader(reader, csvConfig);

            if (!csv.Read() || !csv.ReadHeader())
            {
                throw new InvalidDataException("CSV file doesn't contain header!");
            }
            while (csv.Read())
            {
                string? status = csv[_statusHeader];
                if (!string.IsNullOrEmpty(status)) continue;

                string? thingName = csv[_thingNameHeader];
                if (string.IsNullOrEmpty(thingName)) continue;

                if (result.Add(thingName))
                {
                    yield return thingName;
                }
            }
        }

        #endregion
    }
}
