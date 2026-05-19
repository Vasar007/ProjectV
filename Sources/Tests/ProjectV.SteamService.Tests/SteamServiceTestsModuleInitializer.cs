using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.SteamService.Tests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.SteamService.Tests</c> assembly.
    /// Pre-installs an empty NLog <see cref="LoggingConfiguration" /> so that
    /// production types with a static <c>NLog.Logger</c> field
    /// (<c>SteamApiClient</c>, <c>SteamAppsStorage</c>) do not trigger the
    /// auto-load of <c>NLog.config</c> when the type initialiser runs inside
    /// the test process.
    /// </summary>
    /// <remarks>
    /// Same pattern as <c>TmdbServiceTestsModuleInitializer</c> in the sibling
    /// contract-test project. See its remarks for the underlying NLog 6
    /// <c>concurrentWrites</c> config-file bug that this initializer
    /// works around (tracked in <c>.planning/codebase/CONCERNS.md</c>).
    /// </remarks>
    internal static class SteamServiceTestsModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
