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
    /// <c>throwConfigExceptions="true"</c>. Removing that attribute stopped
    /// the auto-load from throwing and made the workaround no longer
    /// load-bearing for build/test correctness.
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
    /// the NLog 6 auto-load no longer throws on that specific cause, so the
    /// worst-case outcome is a few stray log lines per test process rather
    /// than a load-bearing test correctness risk. Other NLog auto-load
    /// failure modes still exist (<c>throwConfigExceptions="true"</c>
    /// combined with the <c>&lt;extensions&gt;</c> directive will throw if
    /// <c>ProjectV.Logging.dll</c> is absent from the output directory, or
    /// if <c>NLog.config</c> is malformed) — this initializer only guards
    /// against log-file write side effects, not those other failures.
    /// </para>
    /// <para>
    /// Do NOT re-add <c>concurrentWrites="true"</c> to
    /// <c>Sources/Libraries/ProjectV.Logging/NLog.config</c>: NLog 6 dropped
    /// that attribute and any future "I/O optimisation" pass that puts it
    /// back will invalidate the rationale above and reintroduce the
    /// per-assembly auto-load throw the consolidation was built to remove.
    /// If stray production log writes during tests ever become unacceptable
    /// (e.g. CI sandbox isolation), reinstate per-assembly
    /// <c>[ModuleInitializer]</c>s in each test assembly so the empty config
    /// is installed before any production static logger initializes.
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
