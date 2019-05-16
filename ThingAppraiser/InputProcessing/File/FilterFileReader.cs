using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileHelpers;
using CsvHelper;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Provides reading data from files with some filtering. Now this class filters by value of
    /// status field.
    /// </summary>
    public class FilterFileReader : IFileReader
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<FilterFileReader>();

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
        /// <remarks>File must contain "Status" and "Thing Name" columns.</remarks>
        public List<string> ReadFile(string filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            var engine = new FileHelperAsyncEngine<FilterInputFileData>();
            using (engine.BeginReadFile(filename))
            {
                // The engine is IEnumerable.
                result.UnionWith(engine
                                    .Where(data => string.IsNullOrEmpty(data.status))
                                    .Select(data => data.thingName)
                );
            }
            return result.ToList();
        }

        /// <inheritdoc />
        /// <remarks>File must contain "Status" and "Thing Name" columns.</remarks>
        /// <exception cref="InvalidDataException">CSV file doesn't contain header.</exception>
        public List<string> ReadCsvFile(string filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            using (var reader = new StreamReader(filename))
            {
                var csv = new CsvReader(
                    reader, new CsvHelper.Configuration.Configuration { HasHeaderRecord = true }
                );

                if (!csv.Read() || !csv.ReadHeader())
                {
                    var ex = new InvalidDataException("CSV file doesn't contain header!");
                    _logger.Error(ex, "Got CSV file without header.");
                    throw ex;
                }
                while (csv.Read())
                {
                    string status = csv[_statusHeader];
                    if (!string.IsNullOrEmpty(status)) continue;

                    string field = csv[_thingNameHeader];
                    result.Add(field);
                }
            }
            return result.ToList();
        }

        #endregion
    }
}
