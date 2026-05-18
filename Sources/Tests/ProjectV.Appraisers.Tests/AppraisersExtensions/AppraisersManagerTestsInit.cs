using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.Appraisers.Tests.AppraisersExtensions
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.Appraisers.Tests</c> assembly.
    /// Pre-installs an empty NLog <see cref="LoggingConfiguration" /> so that
    /// production types with a static <c>NLog.Logger</c> field
    /// (<see cref="AppraisersManager" /> for example) do not trigger the
    /// auto-load of <c>NLog.config</c> when the type initialiser runs
    /// inside a test process.
    /// </summary>
    /// <remarks>
    /// The repo-wide <c>Sources/Libraries/ProjectV.Logging/NLog.config</c> is
    /// pinned at NLog 6.1.3 yet still uses the <c>concurrentWrites="true"</c>
    /// attribute, which NLog 6 dropped. With
    /// <c>throwConfigExceptions="true"</c>, the auto-load throws
    /// <c>NLog.NLogConfigurationException</c>. This module initializer
    /// short-circuits the auto-load by assigning a benign
    /// <see cref="LoggingConfiguration" /> to
    /// <see cref="NLog.LogManager.Configuration" /> before any production
    /// type is touched. The underlying config-file bug is out-of-scope for
    /// this plan and is tracked in <c>.planning/codebase/CONCERNS.md</c>.
    /// </remarks>
    internal static class TestModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
