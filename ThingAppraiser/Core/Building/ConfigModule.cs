using System.Xml.Linq;

namespace ThingAppraiser.Core.Building
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
                "ConsoleMessageHandlerSetUnicode",
                new XElement("ConsoleMessageHandlerSetUnicode", "true")
            );
        }

        private static void RegisterInputtersConfig()
        {
            _serviceConfigRegistry.RegisterInputter("LocalFileReaderSimple",
                new XElement("LocalFile", new XAttribute("FileReaderLocalFile", "Simple"))
            );
            _serviceConfigRegistry.RegisterInputter("GoogleDriveReaderSimple",
                new XElement("GoogleDrive", new XAttribute("FileReaderGoogleDrive", "Simple"))
            );

            _serviceConfigRegistry.RegisterInputter("LocalFileReaderFilter",
                new XElement("LocalFile", new XAttribute("FileReaderLocalFile", "Filter"))
            );
            _serviceConfigRegistry.RegisterInputter("GoogleDriveReaderFilter",
                new XElement("GoogleDrive", new XAttribute("FileReaderGoogleDrive", "Filter"))
            );
        }

        private static void RegisterCrawlersConfig()
        {
            _serviceConfigRegistry.RegisterCrawler("Tmdb",
                new XElement("TmdbCrawler",
                    new XAttribute("TmdbApiKey", "f7440a70737103fea00fb6e8352a3533"),
                    new XAttribute("TmdbMaxRetryCount", "10")
                )
            );

            _serviceConfigRegistry.RegisterCrawler("Omdb",
                new XElement("OmdbCrawler",
                    new XAttribute("OmdbApiKey", "ba082100")
                )
            );

            _serviceConfigRegistry.RegisterCrawler("Steam",
                new XElement("SteamCrawler",
                    new XAttribute("SteamApiKey", "C484852EA599C02C687E579ABB38D346")
                )
            );
        }

        private static void RegisterAppraisersConfig()
        {
            _serviceConfigRegistry.RegisterAppraiser("TmdbCommon",
                new XElement("TmdbAppraiser")
            );
            _serviceConfigRegistry.RegisterAppraiser("TmdbFuzzy",
                new XElement("FuzzyTmdbAppraiser")
            );

            _serviceConfigRegistry.RegisterAppraiser("OmdbCommon",
                new XElement("OmdbAppraiser")
            );

            _serviceConfigRegistry.RegisterAppraiser("SteamCommon",
                new XElement("SteamAppraiser")
            );
        }

        private static void RegisterOutputtersConfig()
        {
            _serviceConfigRegistry.RegisterOutputter("LocalFileWriter",
                new XElement("LocalFile")
            );
            _serviceConfigRegistry.RegisterOutputter("GoogleDriveWriter",
                new XElement("GoogleDrive")
            );
        }

        private static void RegisterRepositoriesConfig()
        {
            _serviceConfigRegistry.RegisterRepository("BasicInfo",
                new XElement("BasicInfoRepository")
            );

            _serviceConfigRegistry.RegisterRepository("Tmdb",
                new XElement("TmdbMovieRepository")
            );
        }
    }
}
