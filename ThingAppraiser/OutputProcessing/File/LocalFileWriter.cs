using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileHelpers;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Class which can write to files and process output content. Uses FileHelpers library to
    /// delegate all routine work.
    /// </summary>
    public class CLocalFileWriter : IOutputter, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CLocalFileWriter>();

        #region ITagable Implementation

        /// <inheritdoc />
        public String Tag => "LocalFileWriter";

        #endregion


        /// <summary>
        /// Default constructor.
        /// </summary>
        public CLocalFileWriter()
        {
        }

        /// <inheritdoc cref="File.Exists" />
        public static Boolean DoesExistFile(String path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Processes results and writes them to local file.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="filename">Filename to write.</param>
        /// <returns><c>true</c> if no exception occured, <c>false</c> otherwise.</returns>
        private static Boolean WriteFile(List<List<CRatingDataContainer>> results, 
            String filename)
        {
            if (String.IsNullOrEmpty(filename)) return false;

            var engine = new FileHelperAsyncEngine<COuputFileData>
            {
                HeaderText = typeof(COuputFileData).GetCsvHeader()
            };

            using (engine.BeginWriteFile(filename))
            {
                Dictionary<String, List<Double>> converted = ConvertResultsToDict(results);
                engine.WriteNexts(converted.Select(result => new COuputFileData
                {
                    thingName = $"\"{result.Key}\"", // Escape Thing names.
                    ratingValue = result.Value
                }));
            }
            return true;
        }

        /// <summary>
        /// Converts sequential collection <see cref="List{T}" /> to associative collection
        /// <see cref="Dictionary{TKey,TValue}" />.
        /// </summary>
        /// <param name="results">Result to convert.</param>
        /// <returns>Transformed results.</returns>
        private static Dictionary<String, List<Double>> ConvertResultsToDict(
            List<List<CRatingDataContainer>> results)
        {
            // TODO: add additional data structure based on Dictionary<String, (CResultInfo, Int32)>
            // where String is name of the Thing, CResultInfo is meta information and Int32 is place
            // in the rating. If appraiser cannot appraise the Thing, then we would have pair
            // <nameof(Thing), null> (yes, ValueType cannot be null, we'd simply have something
            // similar, i.e. default or special value). Such data structure, I think, can help to
            // decrease memory usage and save sorting order in ratings with meta information.

            var converted = new Dictionary<String, List<Double>>();
            foreach (List<CRatingDataContainer> rating in results)
            {
                foreach (CRatingDataContainer ratingDataContainer in rating)
                {
                    if (converted.TryGetValue(ratingDataContainer.DataHandler.Title,
                        out List<Double> ratingValues))
                    {
                        ratingValues.Add(ratingDataContainer.RatingValue);
                    }
                    else
                    {
                        converted.Add(ratingDataContainer.DataHandler.Title,
                                      new List<Double> { ratingDataContainer.RatingValue });
                    }
                }
            }
            return converted;
        }

        #region IOutputter Implementation

        /// <summary>
        /// Saves results to local file.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="storageName">Filename to write.</param>
        /// <returns><c>true</c> if no exception occured, <c>false</c> otherwise.</returns>
        public Boolean SaveResults(List<List<CRatingDataContainer>> results, String storageName)
        {
            if (String.IsNullOrEmpty(storageName)) return false;

            try
            {
                return WriteFile(results, storageName);
            }
            catch (Exception ex)
            {
                s_logger.Warn(ex, "Couldn't write to the storage.");
                SGlobalMessageHandler.OutputMessage("Couldn't write to the storage. " +
                                                    $"Error: {ex.Message}");
                return false;
            }
        }
        
        #endregion
    }
}
