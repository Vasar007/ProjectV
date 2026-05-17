using ProjectV.Models.Configuration;

namespace ProjectV.Core.ShellBuilders
{
    /// <summary>
    /// Contains identifiers to work with XML configs.
    /// </summary>
    public abstract class ShellBuilderBase
    {
        /// <summary>
        /// Root element of the config.
        /// </summary>
        protected static readonly string _rootElementName = nameof(ConfigurationXml.ShellConfig);

        /// <summary>
        /// Element name for message handler part.
        /// </summary>
        protected static readonly string _messageHandlerParameterName =
            nameof(ConfigurationXml.ShellConfig.MessageHandler);

        /// <summary>
        /// Element name for input manager part.
        /// </summary>
        protected static readonly string _inputManagerParameterName =
            nameof(ConfigurationXml.ShellConfig.InputManager);

        /// <summary>
        /// Attribute name for default input filename of input manager.
        /// </summary>
        protected static readonly string _defaultInStorageNameParameterName =
            nameof(ConfigurationXml.ShellConfig.InputManager.DefaultInStorageName);

        /// <summary>
        /// Element name for crawlers manager part.
        /// </summary>
        protected static readonly string _crawlersManagerParameterName =
            nameof(ConfigurationXml.ShellConfig.CrawlersManager);

        /// <summary>
        /// Attribute name for crawlers output flag of crawlers manager.
        /// </summary>
        protected static readonly string _crawlersOutputParameterName =
            nameof(ConfigurationXml.ShellConfig.CrawlersManager.CrawlersOutputFlag);

        /// <summary>
        /// Element name for appraisers manager part.
        /// </summary>
        protected static readonly string _appraisersManagerParameterName =
            nameof(ConfigurationXml.ShellConfig.AppraisersManager);

        /// <summary>
        /// Attribute name for appraisers output flag of appraisers manager.
        /// </summary>
        protected static readonly string _appraisersOutputParameterName =
            nameof(ConfigurationXml.ShellConfig.AppraisersManager.AppraisersOutputFlag);

        /// <summary>
        /// Element name for output manager part.
        /// </summary>
        protected static readonly string _outputManagerParameterName =
            nameof(ConfigurationXml.ShellConfig.OutputManager);

        /// <summary>
        /// Attribute name for default output filename of output manager.
        /// </summary>
        protected static readonly string _defaultOutStorageNameParameterName =
            nameof(ConfigurationXml.ShellConfig.OutputManager.DefaultOutStorageName);

        /// <summary>
        /// Element name for data base manager part.
        /// </summary>
        protected static readonly string _dataBaseManagerParameterName =
            nameof(ConfigurationXml.ShellConfig.DataBaseManager);


        /// <summary>
        /// Initilizes base class.
        /// </summary>
        protected ShellBuilderBase()
        {
        }
    }
}
