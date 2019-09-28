using ThingAppraiser.Configuration;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class DesktopOptions
    {
        /// <summary>
        /// Page names of all views in main menu.
        /// </summary>
        /// <remarks>
        /// There are contract that page names for browsing service response must be equal to
        /// beautified service names.
        /// </remarks>
        public static class PageNames
        {
            public static string StartPage { get; } = "Start page";

            public static string TmdbPage => ConfigNames.BeautifiedServices.TmdbServiceName;

            public static string OmdbPage => ConfigNames.BeautifiedServices.OmdbServiceName;

            public static string SteamPage => ConfigNames.BeautifiedServices.SteamServiceName;

            public static string ExpertModePage { get; } = "Expert mode";

            public static string ToplistStartPage { get; } = "Toplist editor - start page";

            public static string ToplistEditorPage { get; } = "Toplist editor";

            public static string ContentFinderPage { get; } = "Content finder";
        }

        public static class HintTexts
        {
            public static string HintTextForGoogleDriveDialog { get; } = "Filename on Google Drive";
        }
    }
}
