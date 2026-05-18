using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.Core.Tests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.Core.Tests</c> assembly.
    /// Pre-installs an empty NLog <see cref="LoggingConfiguration" /> so that
    /// production types with a static <c>NLog.Logger</c> field
    /// (<c>Shell</c>, <c>ShellBuilderFromXDocument</c>,
    /// <c>CommunicationServiceClient</c>, …) do not trigger the auto-load of
    /// <c>NLog.config</c> when the type initialiser runs inside the test
    /// process.
    /// </summary>
    /// <remarks>
    /// The repo-wide <c>Sources/Libraries/ProjectV.Logging/NLog.config</c>
    /// declares <c>concurrentWrites="true"</c> on its <c>FileTarget</c> — NLog 6
    /// dropped that attribute. With <c>throwConfigExceptions="true"</c>, the
    /// auto-load throws <c>NLog.NLogConfigurationException</c>. This module
    /// initializer short-circuits the auto-load by assigning a benign
    /// <see cref="LoggingConfiguration" /> to
    /// <see cref="NLog.LogManager.Configuration" /> before any production
    /// type is touched. The underlying config-file bug is out-of-scope for
    /// this plan and is tracked in <c>.planning/codebase/CONCERNS.md</c>.
    /// Same pattern as
    /// <c>ProjectV.Appraisers.Tests.AppraisersExtensions.TestModuleInitializer</c>.
    /// </remarks>
    internal static class CoreTestsModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
