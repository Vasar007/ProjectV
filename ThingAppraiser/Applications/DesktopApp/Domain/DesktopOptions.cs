namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class DesktopOptions
    {
        public static class PageNames
        {
            public static string StartPage { get; } = "Start page";

            public static string TmdbPage { get; } = "TMDb";

            public static string OmdbPage { get; } = "OMDb";

            public static string SteamPage { get; } = "Steam";

            public static string ExpertModePage { get; } = "Expert mode";

            public static string ToplistEditorPage { get; } = "Toplist editor";
        }

        public static class HintTexts
        {
            public static string HintTextForGoogleDriveDialog { get; } = "Filename on Google Drive";
        }
    }
}
