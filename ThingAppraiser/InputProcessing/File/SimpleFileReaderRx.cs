using System;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using FileHelpers;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public class CSimpleFileReaderRx : IFileReaderRx
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CSimpleFileReaderRx>();

        private readonly String _thingNameHeader = "Thing Name";


        public CSimpleFileReaderRx()
        {
        }

        #region IFileReaderAsync Implementation

        public IEnumerable<String> ReadFile(String filename)
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
                        yield return record.thingName;
                    }
                }
            }
        }

        public IEnumerable<String> ReadCsvFile(String filename)
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
                        yield return thingName;
                    }
                }
            }
        }

        #endregion
    }
}
