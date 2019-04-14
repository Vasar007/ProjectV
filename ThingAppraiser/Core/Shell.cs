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
            IO.Output.COutputManager outputManager)
        {
            InputManager = inputManager.ThrowIfNull(nameof(inputManager));
            CrawlersManager = crawlersManager.ThrowIfNull(nameof(crawlersManager));
            AppraisersManager = appraisersManager.ThrowIfNull(nameof(appraisersManager));
            OutputManager = outputManager.ThrowIfNull(nameof(outputManager));
        }

        /// <summary>
        /// Reads Things names from input source.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Collection of The Things names as strings.</returns>
        public List<String> GetThingsNames(String storageName)
        {
            var names = new List<String>();
            if (String.IsNullOrEmpty(storageName)) return names;

            try
            {
                names = InputManager.GetNames(storageName);
                if (names.Count == 0)
                {
                    SGlobalMessageHandler.OutputMessage("Input is empty.");
                    _status = EStatus.Nothing;
                }
                else
                {
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
        /// <returns>Collections of results from crawlers.</returns>
        public List<List<Data.CBasicInfo>> RequestData(List<String> names)
        {
            var results = new List<List<Data.CBasicInfo>>();

            try
            {
                results = CrawlersManager.CollectAllResponses(names);

                _status = results.Count == 0 ? EStatus.Nothing : EStatus.Ok;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during collecting data.");
                _status = EStatus.RequestError;
            }

            // TODO: encapsulate this call to interface.
            CConsoleMessageHandler.PrintResultsToConsole(results);

            return results;
        }

        /// <summary>
        /// Processes the data in accordance with the formulas.
        /// </summary>
        /// <param name="results">Collections of crawlers results to appraise.</param>
        /// <returns>Collections of processed data from appraisers.</returns>
        public List<Data.CRating> AppraiseThings(List<List<Data.CBasicInfo>> results)
        {
            var ratings = new List<Data.CRating>();

            try
            {
                ratings = AppraisersManager.GetAllRatings(results);

                _status = ratings.Count == 0 ? EStatus.Nothing : EStatus.Ok;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during appraising work.");
                _status = EStatus.AppraiseError;
            }

            // TODO: encapsulate this call to interface.
            CConsoleMessageHandler.PrintRatingsToConsole(ratings);

            return ratings;
        }

        /// <summary>
        /// Saves ratings to the output sources.
        /// </summary>
        /// <param name="ratings">Collections of appraisers data to save.</param>
        /// <returns><c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        public Boolean SaveResults(List<Data.CRating> ratings)
        {
            Boolean success = false;

            try
            {
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
            List<List<Data.CBasicInfo>> results = RequestData(names);
            if (_status != EStatus.Ok) return _status;

            // Appraisers component work.
            List<Data.CRating> ratings = AppraiseThings(results);
            if (_status != EStatus.Ok) return _status;

            // Output component work.
            SaveResults(ratings);
            if (_status != EStatus.Ok) return _status;

            s_logger.Info("Shell finished work successfully.");

            return _status;
        }
    }
}
