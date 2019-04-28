using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Key class of service that links the rest of the classes into a single entity.
    /// </summary>
    public sealed class CShell
    {
        /// <summary>
        /// Director which can build shell with the help of specified builder.
        /// </summary>
        public static Building.CShellBuilderDirector ShellBuilderDirector { get; } =
            new Building.CShellBuilderDirector(new Building.CShellBuilderFromConfig());

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CShell>();

        /// <summary>
        /// Represents current status of data processing.
        /// </summary>
        private EStatus _status = EStatus.Nothing;

        /// <summary>
        /// Manager to interact with input data.
        /// </summary>
        public IO.Input.CInputManager InputManager { get; }

        /// <summary>
        /// Manager to collect data about The Things from different sources.
        /// </summary>
        public Crawlers.CCrawlersManager CrawlersManager { get; }

        /// <summary>
        /// Manager to appraise of information collected.
        /// </summary>
        public Appraisers.CAppraisersManager AppraisersManager { get; }

        /// <summary>
        /// Manager to save service results.
        /// </summary>
        public IO.Output.COutputManager OutputManager { get; }

        /// <summary>
        /// Manager to interact with data base.
        /// </summary>
        public DAL.CDataBaseManager DataBaseManager { get; }


        /// <summary>
        /// Default constructor which initialize all managers.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputManager">inputManager</paramref> or
        /// <paramref name="crawlersManager">crawlersManager</paramref> or
        /// <paramref name="appraisersManager">appraisersManager</paramref> or
        /// <paramref name="outputManager">outputManager</paramref> is <c>null</c>.
        /// </exception>
        public CShell(
            IO.Input.CInputManager inputManager,
            Crawlers.CCrawlersManager crawlersManager,
            Appraisers.CAppraisersManager appraisersManager,
            IO.Output.COutputManager outputManager,
            DAL.CDataBaseManager dataBaseManager)
        {
            InputManager = inputManager.ThrowIfNull(nameof(inputManager));
            CrawlersManager = crawlersManager.ThrowIfNull(nameof(crawlersManager));
            AppraisersManager = appraisersManager.ThrowIfNull(nameof(appraisersManager));
            OutputManager = outputManager.ThrowIfNull(nameof(outputManager));
            DataBaseManager = dataBaseManager.ThrowIfNull(nameof(dataBaseManager));
        }

        /// <summary>
        /// Reads Things names from input source.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Collection of The Things names as strings.</returns>
        private List<String> GetThingsNames(String storageName)
        {
            var names = new List<String>();
            if (String.IsNullOrEmpty(storageName)) return names;

            try
            {
                names = InputManager.GetNames(storageName);
                if (names.Count == 0)
                {
                    SGlobalMessageHandler.OutputMessage(
                        $"No Things were found in \"{storageName}\"."
                    );
                    _status = EStatus.Nothing;
                }
                else
                {
                    SGlobalMessageHandler.OutputMessage("Things were successfully gotten.");
                    _status = EStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during input work.");
                _status = EStatus.InputError;
            }

            return names;
        }

        /// <summary>
        /// Sends request and collect data from all services.
        /// </summary>
        /// <param name="names">Collection of The Things names which need to appraise.</param>
        private void RequestData(List<String> names)
        {
            try
            {
                List<List<Data.CBasicInfo>> results = CrawlersManager.CollectAllResponses(names);

                // TODO: encapsulate this call to interface.
                CConsoleMessageHandler.PrintResultsToConsole(results);

                if (results.Count == 0)
                {
                    SGlobalMessageHandler.OutputMessage(
                        "Crawlers have not received responses from services. Result is empty."
                    );
                    _status = EStatus.Nothing;
                }
                else
                {
                    DataBaseManager.PutResultsToDB(results);

                    SGlobalMessageHandler.OutputMessage(
                        "Crawlers have received responses from services."
                    );
                    _status = EStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during collecting data.");
                _status = EStatus.RequestError;
            }
        }

        /// <summary>
        /// Processes the data in accordance with the formulas.
        /// </summary>
        private Data.CRatingsStorage AppraiseThings()
        {
            Data.CRatingsStorage ratingsStorage = null;
            try
            {
                List<Data.CRawDataContainer> results = 
                    DataBaseManager.GetResultsFromDBWithAdditionalInfo();
                Data.CProcessedDataContainer ratings = AppraisersManager.GetAllRatings(results);

                ratingsStorage = ratings.RatingsStorage;

                // TODO: encapsulate this call to interface.
                CConsoleMessageHandler.PrintRatingsToConsole(ratings.GetData());

                if (ratings.GetData().Count == 0)
                {
                    SGlobalMessageHandler.OutputMessage(
                        "Appraisers have not calculated ratings. Result is empty."
                    );
                    _status = EStatus.Nothing;
                }
                else
                {
                    DataBaseManager.DeleteResultAndRatings();
                    DataBaseManager.PutRatingsToDB(ratings);

                    SGlobalMessageHandler.OutputMessage(
                        "Appraisers have calculated ratings successfully."
                    );
                    _status = EStatus.Ok;
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during appraising work.");
                _status = EStatus.AppraiseError;
            }
            return ratingsStorage;
        }

        /// <summary>
        /// Saves ratings to the output sources.
        /// </summary>
        /// <returns><c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        private Boolean SaveResults(Data.CRatingsStorage ratingsStorage)
        {
            Boolean success = false;

            try
            {
                var ratings = DataBaseManager.GetRatingValuesFromDB(ratingsStorage);

                if (OutputManager.SaveResults(ratings, String.Empty))
                {
                    success = true;
                    _status = EStatus.Ok;
                    SGlobalMessageHandler.OutputMessage("Ratings was saved successfully.");
                }
                else
                {
                    _status = EStatus.OutputUnsaved;
                    SGlobalMessageHandler.OutputMessage("Ratings wasn't saved.");
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during output work.");
                _status = EStatus.OutputError;
            }

            return success;
        }

        /// <summary>
        /// Launches the whole cycle of collecting and processing data.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Status value depending on the result of the service.</returns>
        public EStatus Run(String storageName)
        {
            s_logger.Info("Shell started work.");

            // Input component work.
            List<String> names = GetThingsNames(storageName);
            if (_status != EStatus.Ok) return _status;

            // Crawlers component work.
            RequestData(names);
            if (_status != EStatus.Ok) return _status;

            // Appraisers component work.
            Data.CRatingsStorage ratingsStorage = AppraiseThings();
            if (_status != EStatus.Ok) return _status;

            // Output component work.
            SaveResults(ratingsStorage);
            if (_status != EStatus.Ok) return _status;

            s_logger.Info("Shell finished work successfully.");

            return _status;
        }
    }
}
