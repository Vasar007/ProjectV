using System;
using System.Collections.Generic;
using FileHelpers;

namespace ThingAppraiser.IO.Output
{
    public class LocalFileWriter : Outputter
    {
        private static bool WriteFile(List<List<Data.ResultType>> results, string filename)
        {
            var s = typeof(Data.OuputFileData).GetCsvHeader();
            var engine = new FileHelperAsyncEngine<Data.OuputFileData>
            {
                HeaderText = typeof(Data.OuputFileData).GetCsvHeader()
            };
            // engine.HeaderText = engine.GetFileHeader();

            // Write.
            using (engine.BeginWriteFile(filename))
            {
                var converted = ConvertResultsToDict(results);
                // The engine is IEnumerable.
                foreach (var result in converted)
                {
                    var output = new Data.OuputFileData { thingName = result.Key,
                                                          ratingValue = result.Value };
                    engine.WriteNext(output);
                }
            }
            return true;
        }

        private static Dictionary<string, List<float>> ConvertResultsToDict(
            List<List<Data.ResultType>> results)
        {
            var converted = new Dictionary<string, List<float>>();
            foreach (var result in results)
            {
                foreach (var res in result)
                {
                    if (converted.TryGetValue(res.DataHandler.Title, out var ratings))
                    {
                        ratings.Add(res.RatingValue);
                    }
                    else
                    {
                        converted.Add(res.DataHandler.Title, new List<float> { res.RatingValue });
                    }
                }
            }
            return converted;
        }

        public override bool SaveResults(List<List<Data.ResultType>> results, string storageName)
        {
            try
            {
                return WriteFile(results, storageName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Couldn't write to the storage. Error: {ex.Message}");
                return false;
            }
        }
    }
}
