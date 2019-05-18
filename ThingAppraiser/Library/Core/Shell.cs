using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using System.Xml.Linq;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Key class of service that links the rest of the classes into a single entity.
    /// </summary>
    public sealed class Shell
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<Shell>();

        /// <summary>
        /// Represents current status of data processing.
        /// </summary>
        private ServiceStatus _status = ServiceStatus.Nothing;

        /// <summary>
        /// Manager to interact with input data.
        /// </summary>
        public IO.Input.InputManager InputManager { get; }

        /// <summary>
        /// Manager to collect data about The Things from different sources.
        /// </summary>
        public Crawlers.CrawlersManager CrawlersManager { get; }

        /// <summary>
        /// Manager to appraise of information collected.
        /// </summary>
        public Appraisers.AppraisersManager AppraisersManager { get; }

        /// <summary>
        /// Manager to save service results.
        /// </summary>
        public IO.Output.OutputManager OutputManager { get; }

        /// <summary>
        /// Manager to interact with data base.
        /// </summary>
        public DAL.DataBaseManager DataBaseManager { get; }


        /// <summary>
        /// Default constructor which initialize all managers.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputManager" /> or <paramref name="crawlersManager" /> or
        /// <paramref name="appraisersManager" /> or <paramref name="outputManager" /> is 
        /// <c>null</c>.
        /// </exception>
        public Shell(
            IO.Input.InputManager inputManager,
            Crawlers.CrawlersManager crawlersManager,
            Appraisers.AppraisersManager appraisersManager,
            IO.Output.OutputManager outputManager,
            DAL.DataBaseManager dataBaseManager)
        {
            InputManager = inputManager.ThrowIfNull(nameof(inputManager));
            CrawlersManager = crawlersManager.ThrowIfNull(nameof(crawlersManager));
            AppraisersManager = appraisersManager.ThrowIfNull(nameof(appraisersManager));
            OutputManager = outputManager.ThrowIfNull(nameof(outputManager));
            DataBaseManager = dataBaseManager.ThrowIfNull(nameof(dataBaseManager));
        }

        /// <summary>
        /// Creates director with builder which can parse XML config and initialize shell instance.
        /// </summary>
        /// <returns>Helper class to create instances of shell.</returns>
        public static Building.ShellBuilderDirector CreateBuilderDirector(XDocument configuration)
        {
            return new Building.ShellBuilderDirector(
                new Building.CShellBuilderFromXDocument(configuration)
            );
        }

        /// <summary>
        /// Reads Thing names from input source.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Collection of The Thing names as strings.</returns>
        private List<string> GetThingNames(string storageName)
        {
            var names = new List<string>();

            try
            {
                names = InputManager.GetNames(storageName);
                if (names.Count == 0)
                {
                    GlobalMessageHandler.OutputMessage(
                        $"No Things were found in \"{storageName}\"."
                    );
                    _status = ServiceStatus.Nothing;
                }
                else
                {
                    GlobalMessageHandler.OutputMessage("Things were successfully gotten.");
                    _status = ServiceStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during input work.");
                _status = ServiceStatus.InputError;
            }

            return names;
        }

        /// <summary>
        /// Sends request and collect data from all services.
        /// </summary>
        /// <param name="names">Collection of The Things names which need to appraise.</param>
        private void RequestData(List<string> names)
        {
            try
            {
                List<List<BasicInfo>> results = CrawlersManager.CollectAllResponses(names);
                if (results.Count == 0)
                {
                    GlobalMessageHandler.OutputMessage(
                        "Crawlers have not received responses from services. Result is empty."
                    );
                    _status = ServiceStatus.Nothing;
                }
                else
                {
                    DataBaseManager.DeleteData();
                    DataBaseManager.PutResultsToDb(results);

                    GlobalMessageHandler.OutputMessage(
                        "Crawlers have received responses from services."
                    );
                    _status = ServiceStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during collecting data.");
                _status = ServiceStatus.RequestError;
            }
        }

        /// <summary>
        /// Processes the data in accordance with the formulas.
        /// </summary>
        private RatingsStorage AppraiseThings()
        {
            RatingsStorage ratingsStorage = null;
            try
            {
                List<RawDataContainer> results = 
                    DataBaseManager.GetResultsFromDbWithAdditionalInfo();
                ProcessedDataContainer ratings = AppraisersManager.GetAllRatings(results);

                ratingsStorage = ratings.RatingsStorage;
                if (ratings.GetData().Count == 0)
                {
                    GlobalMessageHandler.OutputMessage(
                        "Appraisers have not calculated ratings. Result is empty."
                    );
                    _status = ServiceStatus.Nothing;
                }
                else
                {
                    DataBaseManager.DeleteResultAndRatings();
                    DataBaseManager.PutRatingsToDb(ratings);

                    GlobalMessageHandler.OutputMessage(
                        "Appraisers have calculated ratings successfully."
                    );
                    _status = ServiceStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during appraising work.");
                _status = ServiceStatus.AppraiseError;
            }
            return ratingsStorage;
        }

        /// <summary>
        /// Saves ratings to the output sources.
        /// </summary>
        /// <returns><c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        private bool SaveResults(RatingsStorage ratingsStorage)
        {
            bool success = false;

            try
            {
                var ratings = DataBaseManager.GetRatingValuesFromDb(ratingsStorage);

                if (OutputManager.SaveResults(ratings, string.Empty))
                {
                    success = true;
                    _status = ServiceStatus.Ok;
                    GlobalMessageHandler.OutputMessage("Ratings was saved successfully.");
                }
                else
                {
                    _status = ServiceStatus.OutputUnsaved;
                    GlobalMessageHandler.OutputMessage("Ratings wasn't saved.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during output work.");
                _status = ServiceStatus.OutputError;
            }

            return success;
        }

        /// <summary>
        /// Launches the whole cycle of collecting and processing data.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Status value depending on the result of the service.</returns>
        public ServiceStatus Run(string storageName)
        {
            _logger.Info("Shell started work.");

            // Input component work.
            List<string> names = GetThingNames(storageName);
            if (_status != ServiceStatus.Ok) return _status;

            // Crawlers component work.
            RequestData(names);
            if (_status != ServiceStatus.Ok) return _status;

            // Appraisers component work.
            RatingsStorage ratingsStorage = AppraiseThings();
            if (_status != ServiceStatus.Ok) return _status;

            // Output component work.
            SaveResults(ratingsStorage);
            if (_status != ServiceStatus.Ok) return _status;

            _logger.Info("Shell finished work successfully.");

            return _status;
        }
    }
}
