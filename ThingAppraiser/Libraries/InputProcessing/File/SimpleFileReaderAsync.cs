using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using FileHelpers;
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

        public async Task ReadFile(BufferBlock<string> queue, string filename)
        {
            _logger.Info($"Reading file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();
            using (var engine = new FileHelperAsyncEngine<InputFileData>())
            using (engine.BeginReadFile(filename))
            {
                foreach (var record in engine)
                {
                    if (result.Add(record.thingName))
                    {
                        await queue.SendAsync(record.thingName);
                    }
                }
            }
        }

        public async Task ReadCsvFile(BufferBlock<string> queue, string filename)
        {
            _logger.Info($"Reading CSV file \"{filename}\".");

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<string>();

            using (var reader = new StreamReader(filename))
            using (var csv = new CsvReader(
                       reader, new CsvHelper.Configuration.Configuration { HasHeaderRecord = true }
                  )
            )
            {
                if (!csv.Read() || !csv.ReadHeader())
                {
                    throw new InvalidDataException("CSV file doesn't contain header!");
                }
                while (csv.Read())
                {
                    string thingName = csv[_thingNameHeader];
                    if (result.Add(thingName))
                    {
                        await queue.SendAsync(thingName);
                    }
                }
            }
        }

        #endregion
    }
}
