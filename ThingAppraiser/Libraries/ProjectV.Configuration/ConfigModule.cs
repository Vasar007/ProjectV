using System.Xml.Linq;

namespace ProjectV.Configuration
{
    // TODO: crete common options class with node names.
    public static class ConfigModule
    {
        private static readonly ServiceConfigRegistry _serviceConfigRegistry =
            new ServiceConfigRegistry();


        static ConfigModule()
        {
            RegisterMessageHandlersConfig();
            RegisterInputtersConfig();
            RegisterCrawlersConfig();
            RegisterAppraisersConfig();
            RegisterOutputtersConfig();
            RegisterRepositoriesConfig();
        }

        public static XElement GetConfigForMessageHandlerParameter(string identifier)
        {
            return _serviceConfigRegistry.GetConfigForMessageHandlerParameter(identifier);
        }

        public static XElement GetConfigForInputter(string identifier)
        {
            return _serviceConfigRegistry.GetConfigForInputter(identifier);
        }

        public static XElement GetConfigForCrawler(string identifier)
        {
            return _serviceConfigRegistry.GetConfigForCrawler(identifier);
        }

        public static XElement GetConfigForAppraiser(string identifier)
        {
            return _serviceConfigRegistry.GetConfigForAppraiser(identifier);
        }

        public static XElement GetConfigForOutputter(string identifier)
        {
            return _serviceConfigRegistry.GetConfigForOutputter(identifier);
        }

        public static XElement GetConfigForRepository(string identifier)
        {
            return _serviceConfigRegistry.GetConfigForRepository(identifier);
        }

        private static void RegisterMessageHandlersConfig()
        {
            _serviceConfigRegistry.RegisterMessageHandlerParameter(
                ConfigNames.MessageHandlerParameters.ConsoleMessageHandlerSetUnicodeName,
                new XElement("ConsoleMessageHandlerSetUnicode", "true")
            );
        }

        private static void RegisterInputtersConfig()
        {
            _serviceConfigRegistry.RegisterInputter(
                ConfigNames.Inputters.LocalFileReaderSimpleName,
                new XElement("LocalFile",
                    new XAttribute("FileReaderLocalFile", "Simple")
                )
            );
            _serviceConfigRegistry.RegisterInputter(
                ConfigNames.Inputters.GoogleDriveReaderSimpleName,
                new XElement("GoogleDrive",
                    new XAttribute("FileReaderGoogleDrive", "Simple")
                )
            );

            _serviceConfigRegistry.RegisterInputter(
                ConfigNames.Inputters.LocalFileReaderFilterName,
                new XElement("LocalFile",
                    new XAttribute("FileReaderLocalFile", "Filter")
                )
            );
            _serviceConfigRegistry.RegisterInputter(
                ConfigNames.Inputters.GoogleDriveReaderFilterName,
                new XElement("GoogleDrive",
                    new XAttribute("FileReaderGoogleDrive", "Filter")
                )
            );
        }

        private static void RegisterCrawlersConfig()
        {
            _serviceConfigRegistry.RegisterCrawler(
                ConfigNames.Crawlers.TmdbCrawlerName,
                new XElement("TmdbCrawler",
                    new XAttribute("TmdbApiKey", ConfigOptions.Api.TmdbApiKey),
                    new XAttribute("TmdbMaxRetryCount", "10")
                )
            );

            _serviceConfigRegistry.RegisterCrawler(
                ConfigNames.Crawlers.OmdbCrawlerName,
                new XElement("OmdbCrawler",
                    new XAttribute("OmdbApiKey", ConfigOptions.Api.OmdbApiKey)
                )
            );

            _serviceConfigRegistry.RegisterCrawler(
                ConfigNames.Crawlers.SteamCrawlerName,
                new XElement("SteamCrawler",
                    new XAttribute("SteamApiKey", ConfigOptions.Api.SteamApiKey)
                )
            );
        }

        private static void RegisterAppraisersConfig()
        {
            _serviceConfigRegistry.RegisterAppraiser(
                ConfigNames.Appraisers.TmdbAppraiserCommonName,
                new XElement("TmdbAppraiser")
            );
            _serviceConfigRegistry.RegisterAppraiser(
                ConfigNames.Appraisers.TmdbAppraiserFuzzyName,
                new XElement("FuzzyTmdbAppraiser")
            );

            _serviceConfigRegistry.RegisterAppraiser(
                ConfigNames.Appraisers.OmdbAppraiserCommonName,
                new XElement("OmdbAppraiser")
            );

            _serviceConfigRegistry.RegisterAppraiser(
                ConfigNames.Appraisers.SteamAppraiserCommonName,
                new XElement("SteamAppraiser")
            );
        }

        private static void RegisterOutputtersConfig()
        {
            _serviceConfigRegistry.RegisterOutputter(
                ConfigNames.Outputters.LocalFileWriterName,
                new XElement("LocalFile")
            );
            _serviceConfigRegistry.RegisterOutputter(
                ConfigNames.Outputters.GoogleDriveWriterName,
                new XElement("GoogleDrive")
            );
        }

        private static void RegisterRepositoriesConfig()
        {
            _serviceConfigRegistry.RegisterRepository(
                ConfigNames.Repositories.BasicInfoRepositoryName,
                new XElement("BasicInfoRepository")
            );

            _serviceConfigRegistry.RegisterRepository(
                ConfigNames.Repositories.TmdbMovieRepositoryName,
                new XElement("TmdbMovieRepository")
            );

            _serviceConfigRegistry.RegisterRepository(
                ConfigNames.Repositories.OmdbMovieRepositoryName,
                new XElement("NotImplemented")
            );

            _serviceConfigRegistry.RegisterRepository(
                ConfigNames.Repositories.SteamGameRepositoryName,
                new XElement("NotImplemented")
            );
        }
    }
}
