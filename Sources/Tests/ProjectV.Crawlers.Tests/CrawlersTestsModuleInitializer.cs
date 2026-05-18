using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.Crawlers.Tests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.Crawlers.Tests</c> assembly.
    /// Pre-installs an empty NLog <see cref="LoggingConfiguration" /> so that
    /// production types with a static <c>NLog.Logger</c> field
    /// (<c>CrawlersManager</c>) do not trigger the auto-load of
    /// <c>NLog.config</c> when the type initialiser runs inside the test
    /// process.
    /// </summary>
    /// <remarks>
    /// Same pattern as
    /// <c>ProjectV.Core.Tests.CoreTestsModuleInitializer</c> (introduced in
    /// 02-05) and
    /// <c>ProjectV.Appraisers.Tests.AppraisersExtensions.TestModuleInitializer</c>
    /// (introduced in 02-04). The repo-wide
    /// <c>Sources/Libraries/ProjectV.Logging/NLog.config</c> declares
    /// <c>concurrentWrites="true"</c> on its <c>FileTarget</c> — NLog 6
    /// dropped that attribute, so with
    /// <c>throwConfigExceptions="true"</c> the auto-load throws
    /// <c>NLog.NLogConfigurationException</c>. This initializer short-circuits
    /// the auto-load by installing a benign
    /// <see cref="LoggingConfiguration" />. The underlying config-file bug is
    /// tracked in <c>.planning/codebase/CONCERNS.md</c>.
    /// </remarks>
    internal static class CrawlersTestsModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
