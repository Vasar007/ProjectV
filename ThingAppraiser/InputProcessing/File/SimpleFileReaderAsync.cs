using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using FileHelpers;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public class CSimpleFileReaderAsync : IFileReaderAsync
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CSimpleFileReaderAsync>();

        private readonly String _thingNameHeader = "Thing Name";


        public CSimpleFileReaderAsync()
        {
        }

        #region IFileReaderAsync Implementation

        public async Task ReadFile(BufferBlock<String> queue, String filename)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var result = new HashSet<String>();
            var engine = new FileHelperAsyncEngine<CInputFileData>();
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

        public async Task ReadCsvFile(BufferBlock<String> queue, String filename)
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
