using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.TelegramBotWebService.Tests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.TelegramBotWebService.Tests</c>
    /// assembly. Pre-installs an empty NLog
    /// <see cref="LoggingConfiguration" /> so that
    /// <c>ProjectV.TelegramBotWebService.Program</c>'s static
    /// <c>NLog.Logger _logger</c> field — which the test host's
    /// <see cref="Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory{TStartup}" />
    /// touches when it loads the entry-point assembly — does not trigger
    /// the auto-load of <c>NLog.config</c> at type-initialisation time.
    /// </summary>
    /// <remarks>
    /// The repo-wide <c>Sources/Libraries/ProjectV.Logging/NLog.config</c>
    /// declares <c>concurrentWrites="true"</c> on its <c>FileTarget</c> —
    /// NLog 6 dropped that attribute. With
    /// <c>throwConfigExceptions="true"</c>, the auto-load throws
    /// <c>NLog.NLogConfigurationException</c>. Same workaround as the
    /// <c>ProjectV.Core.Tests</c> / <c>ProjectV.Crawlers.Tests</c> /
    /// <c>ProjectV.OutputProcessing.Tests</c> /
    /// <c>ProjectV.CommunicationWebService.Tests</c> assemblies — fix to
    /// the config file itself is out-of-scope here and is tracked in
    /// <c>.planning/codebase/CONCERNS.md</c>.
    /// </remarks>
    internal static class TelegramBotWebServiceTestsModuleInitializer
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            NLog.LogManager.Configuration = new LoggingConfiguration();
        }
    }
}
