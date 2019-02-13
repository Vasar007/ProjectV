using System.Collections.Generic;
using System.Linq;
using System.IO;
using FileHelpers;
using CsvHelper;

namespace ThingAppraiser.IO.Input
{
    public class LocalFileReader : IInputter
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static List<string> ReadFile(string filename)
        {
            var engine = new FileHelperAsyncEngine<Data.InputFileData>();

            var result = new List<string>();
            // Read.
            using (engine.BeginReadFile(filename))
            {
                // The engine is IEnumerable.
                result.AddRange(engine.Select(data => data.thingName));
            }
            return result;
        }

        private static List<string> ReadCsvFile(string filename)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(filename))
            {
                var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration()
                                        { HasHeaderRecord = true});

                if (!csv.Read() || !csv.ReadHeader())
                {
                    _logger.Error("CSV file didn't contain header!");
                    throw new InvalidDataException("CSV file didn't contain header!");
                }
                while (csv.Read())
                {
                    // By header name
                    var field = csv["Thing Name"];
                    result.Add(field);
                }
            }
            return result;
        }

        private static List<string> ReadRawFile(string filename)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(filename))
            {
                // Scanning name of things and removing special symbols.
                string line;
                while (!((line = reader.ReadLine()) is null)) // Yeah, I know it's ugly.
                {
                    result.Add(line.Trim('\r', '\n', ' '));
                }
            }
            return result;
        }

        public List<string> ReadThingNames(string storageName)
        {
            var result = new List<string>();
            if (storageName.EndsWith(".csv"))
            {
                result = ReadCsvFile(storageName);
            }
            else
            {
                result = ReadFile(storageName);
            }
            return result;
        }
    }
}
