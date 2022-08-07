﻿namespace ProjectV.Options
{
    public static class CommonConstants
    {
        public static string ApplicationName { get; } = "ProjectV";

        public static string TextLogTraceListenerName { get; } = "TextLogTraceListener";

        public static string ConfigFilename { get; } = "config.json";

        /// <summary>
        /// Number of milliseconds that reload will wait before calling Load. This helps
        /// avoid triggering reload before a file is completely written. Default is 250.
        /// </summary>
        public static int ConfigReloadDelay { get; } = 250;

        public static string CommonApplicationData { get; } = "SpecialFolder.CommonApplicationData";

        public static string DefaultLogFilenameExtensions { get; } = ".log";

        public static string DefaultLogFilenameSeparator { get; } = "-";

        public static string DefaultResultFilename { get; } = "results.xlsx";

        public static string NotAvailable { get; } = "N/A";

        public const int MinPort = 0;
        public const int MaxPort = 65535;
    }
}
