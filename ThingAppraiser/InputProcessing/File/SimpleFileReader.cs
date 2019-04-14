using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileHelpers;
using CsvHelper;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Provides simple and common methods to read data from files.
    /// </summary>
    public class CSimpleFileReader : IFileReader
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CSimpleFileReader>();

        /// <summary>
        /// Name of the column with Thing name.
        /// </summary>
        private readonly String _thingNameHeader = "Thing Name";


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CSimpleFileReader()
        {
        }

        #region IFileReader Implementation

        /// <inheritdoc />
        /// <remarks>File must contain "Thing Name" columns.</remarks>
        public List<String> ReadFile(String filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<String>();
            var engine = new FileHelperAsyncEngine<CInputFileData>();
            using (engine.BeginReadFile(filename))
            {
                // The engine is IEnumerable.
                result.UnionWith(engine.Select(data => data.thingName));
            }
            return result.ToList();
        }

        /// <inheritdoc />
        /// <remarks>File must contain "Thing Name" columns.</remarks>
        /// <exception cref="InvalidDataException">CSV file doesn't contain header.</exception>
        public List<String> ReadCsvFile(String filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<String>();
            using (var reader = new StreamReader(filename))
            {
                var csv = new CsvReader(
                    reader, new CsvHelper.Configuration.Configuration { HasHeaderRecord = true }
                );

                if (!csv.Read() || !csv.ReadHeader())
                {
                    var ex = new InvalidDataException("CSV file doesn't contain header!");
                    s_logger.Error(ex, "Got CSV file without header.");
                    throw ex;
                }
                while (csv.Read())
                {
                    String thingName = csv[_thingNameHeader];
                    result.Add(thingName);
                }
            }
            return result.ToList();
        }

        #endregion
    }
}
