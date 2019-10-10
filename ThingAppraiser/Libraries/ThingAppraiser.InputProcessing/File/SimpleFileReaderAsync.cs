using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using FileHelpers;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input.File
{
    public sealed class SimpleFileReaderAsync : IFileReaderAsync
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<SimpleFileReaderAsync>();

        private readonly string _thingNameHeader;


        public SimpleFileReaderAsync(string thingNameHeader = "Thing Name")
        {
            _thingNameHeader = thingNameHeader.ThrowIfNullOrWhiteSpace(nameof(thingNameHeader));
        }

        #region IFileReaderAsync Implementation

        public IEnumerable<string> ReadFile(string filename)
        {
            _logger.Info($"Reading file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            using var engine = new FileHelperAsyncEngine<InputFileData>();
            using (engine.BeginReadFile(filename))
            {
                foreach (InputFileData record in engine)
                {
                    if (result.Add(record.thingName))
                    {
                        yield return record.thingName;
                    }
                }
            }
        }

        public IEnumerable<string> ReadCsvFile(string filename)
        {
            _logger.Info($"Reading CSV file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            using var reader = new StreamReader(filename);
            using var csv = new CsvReader(reader, new Configuration { HasHeaderRecord = true });

            if (!csv.Read() || !csv.ReadHeader())
            {
                throw new InvalidDataException("CSV file doesn't contain header!");
            }
            while (csv.Read())
            {
                string thingName = csv[_thingNameHeader];
                if (result.Add(thingName))
                {
                    yield return thingName;
                }
            }
        }

        #endregion
    }
}
