using System;
using System.Collections.Generic;

namespace ThingAppraiser.Core
{
    public class Shell
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static IMessageHandler MessageHandler { get; set; }

        public enum Status { Nothing, Ok, Error, InputError, RequestError,
                             AppraiseError, OutputError };

        private IO.Input.InputManager _inputManager;
        private Crawlers.CrawlersManager _crawlersManager;
        private Appraisers.AppraisersManager _appraisersManager;
        private IO.Output.OutputManager _outputManager;

        // TODO: add config parameters and change constructor body.
        public Shell()
        {
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

        public List<List<Data.DataHandler>> RequestData(List<string> names)
        {
            var results = _crawlersManager.GetAllData(names);
            ConsoleMessageHandler.PrintResultsToConsole(results);
            return results;
        }

        public List<List<Data.ResultType>> AppraiseThings(List<List<Data.DataHandler>> results)
        {
            var ratings = _appraisersManager.GetAllRatings(results);
            ConsoleMessageHandler.PrintRatingsToConsole(ratings);
            return ratings;
        }

        public bool SaveResults(List<List<Data.ResultType>> ratings)
        {
            if (_outputManager.SaveResults(ratings))
            {
                OutputMessage("Ratings was saved to the file.");
                return true;
            }
            else
            {
                OutputMessage("Ratings wasn't saved to the file.");
                return false;
            }
        }

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

        public static string GetMessage()
        {
            return MessageHandler?.GetMessage();
        }

        public static void OutputMessage(string message)
        {
            MessageHandler?.OutputMessage(message);
        }
    }
}
