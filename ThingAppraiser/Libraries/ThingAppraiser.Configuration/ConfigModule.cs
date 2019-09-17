using System.Xml.Linq;

namespace ThingAppraiser.Configuration
{
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
                ConfigOptions.MessageHandlerParameters.ConsoleMessageHandlerSetUnicodeName,
                new XElement("ConsoleMessageHandlerSetUnicode", "true")
            );
        }

        private static void RegisterInputtersConfig()
        {
            _serviceConfigRegistry.RegisterInputter(
                ConfigOptions.Inputters.LocalFileReaderSimpleName,
                new XElement("LocalFile",
                    new XAttribute("FileReaderLocalFile", "Simple")
                )
            );
            _serviceConfigRegistry.RegisterInputter(
                ConfigOptions.Inputters.GoogleDriveReaderSimpleName,
                new XElement("GoogleDrive",
                    new XAttribute("FileReaderGoogleDrive", "Simple")
                )
            );

            _serviceConfigRegistry.RegisterInputter(
                ConfigOptions.Inputters.LocalFileReaderFilterName,
                new XElement("LocalFile",
                    new XAttribute("FileReaderLocalFile", "Filter")
                )
            );
            _serviceConfigRegistry.RegisterInputter(
                ConfigOptions.Inputters.GoogleDriveReaderFilterName,
                new XElement("GoogleDrive",
                    new XAttribute("FileReaderGoogleDrive", "Filter")
                )
            );
        }

        private static void RegisterCrawlersConfig()
        {
            _serviceConfigRegistry.RegisterCrawler(
                ConfigOptions.Crawlers.TmdbCrawlerName,
                new XElement("TmdbCrawler",
                    new XAttribute("TmdbApiKey", Options.TmdbApiKey),
                    new XAttribute("TmdbMaxRetryCount", "10")
                )
            );

            _serviceConfigRegistry.RegisterCrawler(
                ConfigOptions.Crawlers.OmdbCrawlerName,
                new XElement("OmdbCrawler",
                    new XAttribute("OmdbApiKey", Options.OmdbApiKey)
                )
            );

            _serviceConfigRegistry.RegisterCrawler(
                ConfigOptions.Crawlers.SteamCrawlerName,
                new XElement("SteamCrawler",
                    new XAttribute("SteamApiKey", Options.SteamApiKey)
                )
            );
        }

        private static void RegisterAppraisersConfig()
        {
            _serviceConfigRegistry.RegisterAppraiser(
                ConfigOptions.Appraisers.TmdbAppraiserCommonName,
                new XElement("TmdbAppraiser")
            );
            _serviceConfigRegistry.RegisterAppraiser(
                ConfigOptions.Appraisers.TmdbAppraiserFuzzyName,
                new XElement("FuzzyTmdbAppraiser")
            );

            _serviceConfigRegistry.RegisterAppraiser(
                ConfigOptions.Appraisers.OmdbAppraiserCommonName,
                new XElement("OmdbAppraiser")
            );

            _serviceConfigRegistry.RegisterAppraiser(
                ConfigOptions.Appraisers.SteamAppraiserCommonName,
                new XElement("SteamAppraiser")
            );
        }

        private static void RegisterOutputtersConfig()
        {
            _serviceConfigRegistry.RegisterOutputter(
                ConfigOptions.Outputters.LocalFileWriterName,
                new XElement("LocalFile")
            );
            _serviceConfigRegistry.RegisterOutputter(
                ConfigOptions.Outputters.GoogleDriveWriterName,
                new XElement("GoogleDrive")
            );
        }

        private static void RegisterRepositoriesConfig()
        {
            _serviceConfigRegistry.RegisterRepository(
                ConfigOptions.Repositories.BasicInfoRepositoryName,
                new XElement("BasicInfoRepository")
            );

            _serviceConfigRegistry.RegisterRepository(
                ConfigOptions.Repositories.TmdbMovieRepositoryName,
                new XElement("TmdbMovieRepository")
            );

            _serviceConfigRegistry.RegisterRepository(
                ConfigOptions.Repositories.OmdbMovieRepositoryName,
                new XElement("NotImplemented")
            );

            _serviceConfigRegistry.RegisterRepository(
                ConfigOptions.Repositories.SteamGameRepositoryName,
                new XElement("NotImplemented")
            );
        }
    }
}
