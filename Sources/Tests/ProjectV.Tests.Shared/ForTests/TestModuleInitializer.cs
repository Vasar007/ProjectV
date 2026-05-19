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
    /// code first touches a Tests.Shared symbol. Even when a production type
    /// with a static NLog logger is touched before Tests.Shared loads, the
    /// fallout is at most a handful of early log lines before the empty
    /// config takes over — acceptable, because the root-cause auto-load
    /// failure is already gone.
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
