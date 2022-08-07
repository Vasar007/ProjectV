﻿using ProjectV.Configuration;

namespace ProjectV.DesktopApp.Domain
{
    internal static class DesktopOptions
    {
        public static string Title { get; } = "Project V";

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

            public static string ToplistEditorPage { get; } = "Toplist editor";

            public static string ContentDirectoriesPage { get; } = "Content directories";

            public static string SettingsPage { get; } = "Settings";
        }

        public static class HintTexts
        {
            public static string HintTextForGoogleDriveDialog { get; } = "Filename on Google Drive";
        }

        public static class BindingNames
        {
            public static string ToplistHeader { get; } = "ToplistHeader";
            public static string ToplistEditor { get; } = "ToplistEditor";
            public static string ContentDirectoriesHeader { get; } = "ContentDirectoriesHeader";
            public static string ContentDirectoriesResults { get; } = "ContentDirectoriesResults";
        }
    }
}
