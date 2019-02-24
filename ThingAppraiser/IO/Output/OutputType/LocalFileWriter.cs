using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;

namespace ThingAppraiser.IO.Output
{
    public class LocalFileWriter : IOutputter
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static bool WriteFile(List<List<Data.ResultType>> results, string filename)
        {
            var engine = new FileHelperAsyncEngine<Data.OuputFileData>
            {
                HeaderText = typeof(Data.OuputFileData).GetCsvHeader()
            };

            // Write.
            using (engine.BeginWriteFile(filename))
            {
                var converted = ConvertResultsToDict(results);
                engine.WriteNexts(converted.Select(result => new Data.OuputFileData
                {
                    thingName = $"\"{result.Key}\"", // Escape Thing names.
                    ratingValue = result.Value
                }));
            }
            return true;
        }

        // TODO: add additional data structure based on Dictionary<string, (Data.ResultType, uint)>
        // where string is name of the Thing, ResultType is meta information and uint is place in
        // the rating. If appraiser cannot appraise the Thing, then we would have pair
        // <nameof(Thing), null>. Such data structure, I think, can help to decrease memory usage
        // and save sorting order in ratings with meta information.
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

        public bool SaveResults(List<List<Data.ResultType>> results, string storageName)
        {
            try
            {
                return WriteFile(results, storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Couldn't write to the storage.");
                Core.Shell.OutputMessage($"Couldn't write to the storage. Error: {ex.Message}");
                return false;
            }
        }
    }
}
