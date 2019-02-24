using System;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Represents methods to create classes for service depend on config parameters.
    /// </summary>
    public static class Builder
    {
        #region Static Fields

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Create message handler instance depend on parameter value (could be read from config 
        /// file).
        /// </summary>
        /// <param name="messageHandlerName">Name of the message handler class to create.</param>
        /// <returns>Fully initialized instance of message handler class.</returns>
        public static IMessageHandler CreateMessageHandlerByName(string messageHandlerName)
        {
            switch (messageHandlerName)
            {
                case "ConsoleMessageHandler":
                    return new ConsoleMessageHandler();

                default:
                    var ex = new ArgumentException(
                        "Couldn't recognize input type specified in config.",
                        nameof(messageHandlerName)
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {messageHandlerName}");
                    throw ex;
            }
        }

        /// <summary>
        /// Create inputter instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="inputType">Type of the input.</param>
        /// <returns>Fully initialized instance of inputter interface.</returns>
        public static IO.Input.IInputter CreateInputter(string inputType)
        {
            switch (inputType)
            {
                case "LocalFile":
                    return new IO.Input.LocalFileReader();

                case "GoogleDrive":
                    return new IO.Input.GoogleDriveReader();

                default:
                    var ex = new ArgumentException(
                        "Couldn't recognize input type specified in config.", nameof(inputType)
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {inputType}");
                    throw ex;
            }
        }

        /// <summary>
        /// Create crawler instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="crawlerName">Name of the crawler class to create.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        public static Crawlers.Crawler CreateCrawlerByName(string crawlerName)
        {
            switch (crawlerName)
            {
                case "TMDBCrawler":
                    return new Crawlers.TMDBCrawler(
                        ConfigParser.GetValueByParameterKey("TMDBAPIKey"),
                        ConfigParser.GetValueByParameterKey("SearchUrl"),
                        ConfigParser.GetValueByParameterKey<int>("RequestsPerTime"),
                        ConfigParser.GetValueByParameterKey("GoodStatusCode"),
                        ConfigParser.GetValueByParameterKey<int>("LimitAttempts"),
                        ConfigParser.GetValueByParameterKey<int>("MillisecondsTimeout")
                    );

                default:
                    var ex = new ArgumentException(
                        "Couldn't recognize crawler type specified in config.", nameof(crawlerName)
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {crawlerName}");
                    throw ex;
            }
        }

        /// <summary>
        /// Create appraiser instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="appraiserName">Name of the appraiser class to create.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        public static Appraisers.Appraiser CreateAppraiserByName(string appraiserName)
        {
            switch (appraiserName)
            {
                case "TMDBAppraiser":
                    return new Appraisers.TMDBAppraiser();

                default:
                    var ex = new ArgumentException(
                        "Couldn't recognize appraiser type specified in config.",
                        nameof(appraiserName)
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {appraiserName}");
                    throw ex;
            }
        }

        /// <summary>
        /// Create outputter instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="outputType">Type of the output.</param>
        /// <returns>Fully initialized instance of outputter interface.</returns>
        public static IO.Output.IOutputter CreateOutputter(string outputType)
        {
            switch (outputType)
            {
                case "LocalFile":
                    return new IO.Output.LocalFileWriter();

                case "GoogleDrive":
                    return new IO.Output.GoogleDriveWriter();

                default:
                    var ex = new ArgumentException(
                        "Couldn't recognize output type specified in config.", nameof(outputType)
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {outputType}");
                    throw ex;
            }
        }

        #endregion
    }
}
