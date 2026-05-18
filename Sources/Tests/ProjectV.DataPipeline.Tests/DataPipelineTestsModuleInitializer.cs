using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.DataPipeline.Tests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.DataPipeline.Tests</c>
    /// assembly. Pre-installs an empty NLog
    /// <see cref="LoggingConfiguration" /> so that production types with a
    /// static <c>NLog.Logger</c> field (<c>TaskWrapper</c>, and downstream
    /// production types pulled in via <c>InputManager</c> +
    /// <c>CrawlersManager</c> + <c>AppraisersManager</c> +
    /// <c>OutputManager</c>) do not trigger the auto-load of
    /// <c>NLog.config</c> when the type initialiser runs inside the test
    /// process.
    /// </summary>
    /// <remarks>
    /// Same workaround as <c>ProjectV.Core.Tests.CoreTestsModuleInitializer</c>
    /// (introduced in 02-05) — the NLog 6 / <c>concurrentWrites</c> config
    /// bug is tracked in <c>.planning/codebase/CONCERNS.md</c>.
    /// </remarks>
    internal static class DataPipelineTestsModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
