using System;
using System.Collections.Generic;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Key class of service that links the rest of the classes into a single entity.
    /// </summary>
    public class Shell
    {
        #region Nested Types

        /// <summary>
        /// Collections of status values.
        /// </summary>
        public enum Status { Nothing, Ok, Error, InputError, RequestError,
                             AppraiseError, OutputError };

        #endregion

        #region Const & Static Fields

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Const & Static Properties

        /// <summary>
        /// Message handler to control all communications with service.
        /// </summary>
        public static IMessageHandler MessageHandler { get; set; }

        #endregion

        #region Class Fields

        /// <summary>
        /// Manager to interact with input data.
        /// </summary>
        private IO.Input.InputManager _inputManager;

        /// <summary>
        /// Manager to collect data about The Things from different sources.
        /// </summary>
        private Crawlers.CrawlersManager _crawlersManager;

        /// <summary>
        /// Manager to appraise of informations collected.
        /// </summary>
        private Appraisers.AppraisersManager _appraisersManager;

        /// <summary>
        /// Manager to save service results.
        /// </summary>
        private IO.Output.OutputManager _outputManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Shell()
        {
            // TODO: add config parameters and change constructor body.
            MessageHandler = new ConsoleMessageHandler();

            _inputManager = new IO.Input.InputManager(new IO.Input.GoogleDriveReader()); // LocalFileReader
            _crawlersManager = new Crawlers.CrawlersManager
            {
                new Crawlers.TMDBCrawler()
            };
            _appraisersManager = new Appraisers.AppraisersManager
            {
                new Appraisers.TMDBAppraiser()
            };
            _outputManager = new IO.Output.OutputManager(new IO.Output.GoogleDriveWriter()); // LocalFileWriter
        }

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Read Things names from input source set in the config.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Collection of The Things names as strings.</returns>
        public List<string> GetThingsNames(string storageName)
        {
            var names = new List<string>();
            if (string.IsNullOrEmpty(storageName)) return names;

            names = _inputManager.GetNames(storageName);
            if (names.Count == 0)
            {
                OutputMessage("Input is empty.");
            }

            return names;
        }

        /// <summary>
        /// Send request and collect data from all services.
        /// </summary>
        /// <param name="names">Collection of The Things names which need to appraise.</param>
        /// <returns>Collections of results from crawlers.</returns>
        public List<List<Data.DataHandler>> RequestData(List<string> names)
        {
            var results = _crawlersManager.CollectAllResponses(names);
            // TODO: encapsulate this call to interface.
            ConsoleMessageHandler.PrintResultsToConsole(results);
            return results;
        }

        /// <summary>
        /// Process the data in accordance with the formulas set in the configuration.
        /// </summary>
        /// <param name="results">Collections of crawlers results to appraise.</param>
        /// <returns>Collections of processed data from appraisers.</returns>
        public List<List<Data.ResultType>> AppraiseThings(List<List<Data.DataHandler>> results)
        {
            var ratings = _appraisersManager.GetAllRatings(results);
            // TODO: encapsulate this call to interface.
            ConsoleMessageHandler.PrintRatingsToConsole(ratings);
            return ratings;
        }

        /// <summary>
        /// Save ratings to the output sources set in the config.
        /// </summary>
        /// <param name="ratings">Collections of appraisers data to save.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool SaveResults(List<List<Data.ResultType>> ratings)
        {
            if (_outputManager.SaveResults(ratings))
            {
                OutputMessage("Ratings was saved to the file.");
                return true;
            }

            OutputMessage("Ratings wasn't saved to the file.");
            return false;
        }

        /// <summary>
        /// Launch the whole cycle of collecting and processing data.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Status value depending on the result of the service.</returns>
        public Status Run(string storageName)
        {
            _logger.Info("Shell started work.");

            if (string.IsNullOrEmpty(storageName)) return Status.Error;

            // ------------------------------------------------------------------------------------

            List<string> names;
            try
            {
                names = GetThingsNames(storageName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during input work.");
                return Status.InputError;
            }
            if (names.Count == 0) return Status.Nothing;

            // ------------------------------------------------------------------------------------

            List<List<Data.DataHandler>> results;
            try
            {
                results = RequestData(names);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during collecting data.");
                return Status.RequestError;
            }
            if (results.Count == 0) return Status.Nothing;

            // ------------------------------------------------------------------------------------

            List<List<Data.ResultType>> ratings;
            try
            {
                ratings = AppraiseThings(results);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during appraising work.");
                return Status.AppraiseError;
            }
            if (ratings.Count == 0) return Status.Nothing;

            // ------------------------------------------------------------------------------------

            try
            {
                SaveResults(ratings);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during output work.");
                return Status.OutputError;
            }

            // ------------------------------------------------------------------------------------

            _logger.Info("Shell finished work successfully.");
            return Status.Ok;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Get message from input providing by message handler.
        /// </summary>
        /// <returns>Input message.</returns>
        public static string GetMessage()
        {
            return MessageHandler?.GetMessage();
        }

        /// <summary>
        /// Print message to the output source providing be message handler.
        /// </summary>
        /// <param name="message">Message to output.</param>
        public static void OutputMessage(string message)
        {
            MessageHandler?.OutputMessage(message);
        }

        #endregion
    }
}
