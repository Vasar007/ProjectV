namespace ThingAppraiser.Core.Building
{
    public static class ConfigOptions
    {
        public static class MessageHandlerParameters
        {
            public static string ConsoleMessageHandlerSetUnicodeName { get; } =
                "ConsoleMessageHandlerSetUnicode";
        }

        public static class Inputters
        {
            public static string LocalFileReaderSimpleName { get; } = "LocalFileReaderSimple";

            public static string GoogleDriveReaderSimpleName { get; } = "GoogleDriveReaderSimple";

            public static string LocalFileReaderFilterName { get; } = "LocalFileReaderFilter";

            public static string GoogleDriveReaderFilterName { get; } = "GoogleDriveReaderFilter";
        }

        public static class Crawlers
        {
            public static string TmdbCrawlerName { get; } = "Tmdb";

            public static string OmdbCrawlerName { get; } = "Omdb";

            public static string SteamCrawlerName { get; } = "Steam";
        }

        public static class Appraisers
        {
            public static string TmdbAppraiserCommonName { get; } = "TmdbCommon";

            public static string TmdbAppraiserFuzzyName { get; } = "TmdbFuzzy";

            public static string OmdbAppraiserCommonName { get; } = "OmdbCommon";

            public static string SteamAppraiserCommonName { get; } = "SteamCommon";
        }

        public static class Outputters
        {
            public static string LocalFileWriterName { get; } = "LocalFileWriter";

            public static string GoogleDriveWriterName { get; } = "GoogleDriveWriter";
        }

        /// <summary>
        /// Repository names in service registry.
        /// </summary>
        /// <remarks>
        /// There are contract that all repositories (except base) must have same name as
        /// appropriate crawlers.
        /// </remarks>
        public static class Repositories
        {
            public static string BasicInfoRepositoryName { get; } = "BasicInfo";

            public static string TmdbMovieRepositoryName => Crawlers.TmdbCrawlerName;

            public static string OmdbMovieRepositoryName => Crawlers.OmdbCrawlerName;

            public static string SteamGameRepositoryName => Crawlers.SteamCrawlerName;
        }
    }
}
