using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.InputProcessing.Tests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.InputProcessing.Tests</c>
    /// assembly. Pre-installs an empty NLog
    /// <see cref="LoggingConfiguration" /> so that the production
    /// <c>InputManager</c> type (which holds a
    /// <c>private static readonly ProjectV.Logging.ILogger _logger =
    /// LoggerFactory.CreateLoggerFor&lt;InputManager&gt;()</c> field) does
    /// not trigger the auto-load of <c>NLog.config</c> when the type
    /// initialiser runs inside the test process.
    /// </summary>
    /// <remarks>
    /// Same pattern as
    /// <c>ProjectV.Core.Tests.CoreTestsModuleInitializer</c> (02-05) and
    /// <c>ProjectV.Crawlers.Tests.CrawlersTestsModuleInitializer</c> (02-06).
    /// The repo-wide <c>Sources/Libraries/ProjectV.Logging/NLog.config</c>
    /// declares <c>concurrentWrites="true"</c> on its <c>FileTarget</c> —
    /// NLog 6 dropped that attribute, so with
    /// <c>throwConfigExceptions="true"</c> the auto-load throws
    /// <c>NLog.NLogConfigurationException</c>. This initializer
    /// short-circuits the auto-load by installing a benign
    /// <see cref="LoggingConfiguration" />. The underlying config-file bug
    /// is tracked in <c>.planning/codebase/CONCERNS.md</c>.
    /// </remarks>
    internal static class InputProcessingTestsModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
