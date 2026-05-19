using System.Runtime.CompilerServices;
using NLog.Config;

namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Module initializer for the <c>ProjectV.Tests.Shared</c> assembly.
    /// Pre-installs an empty NLog <see cref="LoggingConfiguration" /> so that
    /// production types with a static <c>NLog.Logger</c> field do not write
    /// log files to <c>${CommonApplicationData}/ProjectV/logs/</c> during a
    /// test run.
    /// </summary>
    /// <remarks>
    /// <para>
    /// History: each of the 12 downstream test assemblies used to declare its
    /// own private <c>[ModuleInitializer]</c> with this same body. The
    /// duplication was a workaround for the NLog 6 auto-load failure caused
    /// by <c>concurrentWrites="true"</c> in
    /// <c>Sources/Libraries/ProjectV.Logging/NLog.config</c>, combined with
    /// <c>throwConfigExceptions="true"</c>. Plan 02-13 removed that
    /// attribute, so the auto-load no longer throws and the workaround
    /// stopped being load-bearing for build/test correctness.
    /// </para>
    /// <para>
    /// This single hoisted initializer remains for a softer reason: tests
    /// should not litter the host's production log directory with stray
    /// entries. Tests.Shared is referenced (and globally-used) by every C#
    /// test assembly, so its module initializer fires when downstream test
    /// code first touches a Tests.Shared symbol. Because the
    /// <c>global using</c> directive in <c>Usings/SharedUsings.cs</c> is
    /// compile-time only (the C# compiler resolves it without forcing the
    /// referenced assembly to load), there is a race window between process
    /// start and the first Tests.Shared symbol use. During that window any
    /// production type with a static <c>NLog.Logger</c> field
    /// (<c>CrawlersManager</c>, <c>OutputManager</c>, etc.) can write a
    /// handful of early log lines into <c>${CommonApplicationData}/ProjectV/logs/</c>
    /// before the empty <see cref="LoggingConfiguration" /> takes over.
    /// </para>
    /// <para>
    /// This trade-off is accepted intentionally: with the
    /// <c>concurrentWrites="true"</c> attribute removed from <c>NLog.config</c>
    /// the auto-load no longer throws, so the worst-case outcome is a few
    /// stray log lines per test process rather than a load-bearing test
    /// correctness risk. If a future requirement makes stray production log
    /// writes during tests unacceptable (e.g. CI sandbox isolation), reinstate
    /// per-assembly <c>[ModuleInitializer]</c>s or force-load Tests.Shared
    /// from a startup hook before any production type initialises.
    /// </para>
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
