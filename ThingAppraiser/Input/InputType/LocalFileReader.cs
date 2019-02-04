using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using FileHelpers;
using CsvHelper;

namespace ThingAppraiser.Input
{
    public class LocalFileReader : Inputter
    {
        [DelimitedRecord(","), IgnoreEmptyLines(true), IgnoreFirst(1)]
        private class FileData
        {
            [FieldOrder(1), FieldTitle("Thing Name")]
            public string thingName = default(string); // Default assignement to remove warning.
        }

        private List<string> ReadFile(string filename)
        {
            var engine = new FileHelperAsyncEngine<FileData>();

            var result = new List<string>();
            // Read.
            using (engine.BeginReadFile(filename))
            {
                // The engine is IEnumerable.
                result.AddRange(engine.Select(data => data.thingName));
            }
            return result;
        }

        private List<string> ReadCsvFile(string filename)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(filename))
            {
                var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration()
                                        { HasHeaderRecord = true});

                if (!csv.Read() || !csv.ReadHeader())
                {
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

        private List<string> ReadRawFile(string filename)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(filename))
            {
                // Scanning name of things and removing special symbols.
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(line.Trim('\r', '\n', ' '));
                }
            }
            return result;
        }

        public override List<string> ReadNames(string storageName)
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
