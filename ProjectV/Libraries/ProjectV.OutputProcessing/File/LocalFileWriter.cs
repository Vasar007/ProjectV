using System;
using System.Collections.Generic;
using System.Linq;
using FileHelpers;
using ProjectV.Logging;
using ProjectV.Communication;
using ProjectV.Models.Internal;

namespace ProjectV.IO.Output.File
{
    /// <summary>
    /// Class which can write to files and process output content. Uses FileHelpers library to
    /// delegate all routine work.
    /// </summary>
    public sealed class LocalFileWriter : IOutputter, IOutputterBase, ITagable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<LocalFileWriter>();

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = "LocalFileWriter";

        #endregion


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public LocalFileWriter()
        {
        }

        /// <inheritdoc cref="System.IO.File..Exists" />
        public static bool DoesExistFile(string path)
        {
            return System.IO.File.Exists(path);
        }

        #region IOutputter Implementation

        /// <summary>
        /// Saves results to local file.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="storageName">Filename to write.</param>
        /// <returns><c>true</c> if no exception occured, <c>false</c> otherwise.</returns>
        public bool SaveResults(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string storageName)
        {
            if (string.IsNullOrEmpty(storageName)) return false;

            try
            {
                return WriteFile(results, storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Couldn't write to the storage.");
                GlobalMessageHandler.OutputMessage($"Couldn't write to the storage. Error: {ex}");
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Processes results and writes them to local file.
        /// </summary>
        /// <param name="results">Results to save.</param>
        /// <param name="filename">Filename to write.</param>
        /// <returns><c>true</c> if no exception occured, <c>false</c> otherwise.</returns>
        private static bool WriteFile(IReadOnlyList<IReadOnlyList<RatingDataContainer>> results,
            string filename)
        {
            if (string.IsNullOrEmpty(filename)) return false;

            using (var engine = new FileHelperAsyncEngine<OuputFileData>
                  {
                      HeaderText = typeof(OuputFileData).GetCsvHeader()
                  }
            )
            using (engine.BeginWriteFile(filename))
            {
                Dictionary<string, IList<double>> converted = ConvertResultsToDict(results);
                engine.WriteNexts(converted.Select(result => new OuputFileData
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
        private static Dictionary<string, IList<double>> ConvertResultsToDict(
            IReadOnlyList<IReadOnlyList<RatingDataContainer>> results)
        {
            // TODO: add additional data structure based on Dictionary<string, (CResultInfo, Int32)>
            // where string is name of the Thing, CResultInfo is meta information and Int32 is place
            // in the rating. If appraiser cannot appraise the Thing, then we would have pair
            // <nameof(Thing), null> (yes, ValueType cannot be null, we'd simply have something
            // similar, i.e. default or special value). Such data structure, I think, can help to
            // decrease memory usage and save sorting order in ratings with meta information.

            var converted = new Dictionary<string, IList<double>>();
            foreach (IReadOnlyList<RatingDataContainer> rating in results)
            {
                foreach (RatingDataContainer ratingDataContainer in rating)
                {
                    if (converted.TryGetValue(ratingDataContainer.DataHandler.Title,
                                              out IList<double> ratingValues))
                    {
                        ratingValues.Add(ratingDataContainer.RatingValue);
                    }
                    else
                    {
                        converted.Add(ratingDataContainer.DataHandler.Title,
                                      new List<double> { ratingDataContainer.RatingValue });
                    }
                }
            }
            return converted;
        }
    }
}
