using System;
using System.Linq;
using System.Threading;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Webhook
{
    /// <summary>
    /// Temporary diagnostic helper — captures any unhandled exception
    /// surfaced by the test host's request pipeline so failing scenario
    /// tests can include the full stack in the AwesomeAssertions reason
    /// string. The production
    /// <c>ExceptionMiddleware</c> swallows the exception details into a
    /// generic HTTP 500 body and only logs the stack via NLog, so we
    /// attach a <see cref="MemoryTarget" /> to NLog to read those logs
    /// back from the test.
    /// </summary>
    /// <remarks>
    /// Used by <see cref="TelegramWebhookScenarioBaseTest" />. The capture
    /// is pipeline-local — every test grabs and clears the value at the
    /// start of its act phase so a previous test's exception never leaks
    /// into the assertion of a later test.
    /// </remarks>
    internal static class CapturedException
    {
        private static readonly object _gate = new object();
        private static MemoryTarget? _memoryTarget;
        private static Exception? _last;

        /// <summary>
        /// Gets the most recently captured pipeline exception, or
        /// <c>null</c> if no request has thrown since the last
        /// <see cref="Clear" /> call.
        /// </summary>
        public static Exception? Last => Volatile.Read(ref _last);

        /// <summary>
        /// Gets a snapshot of every log line captured by the
        /// <see cref="MemoryTarget" />. May be empty if the production
        /// code did not log on the request path; this includes NLog
        /// formatting plus exception stacks per the layout below.
        /// </summary>
        public static System.Collections.Generic.IReadOnlyList<string> LogLines
        {
            get
            {
                lock (_gate)
                {
                    if (_memoryTarget is null) return Array.Empty<string>();
                    return _memoryTarget.Logs.ToArray();
                }
            }
        }

        /// <summary>
        /// Attaches the diagnostic <see cref="MemoryTarget" /> to the
        /// NLog configuration. Safe to call multiple times — the second
        /// and later calls are no-ops.
        /// </summary>
        public static void EnsureNLogMemoryTarget()
        {
            lock (_gate)
            {
                if (_memoryTarget is not null) return;

                _memoryTarget = new MemoryTarget
                {
                    Name = "projectv-test-capture",
                    Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}|${exception:format=ToString}"
                };

                var config = LogManager.Configuration ?? new LoggingConfiguration();
                config.AddTarget(_memoryTarget);
                config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, _memoryTarget);
                LogManager.Configuration = config;
            }
        }

        /// <summary>
        /// Captures the supplied exception so it surfaces in failing
        /// assertion messages.
        /// </summary>
        /// <param name="exception">Exception to capture.</param>
        public static void Capture(Exception exception)
        {
            Volatile.Write(ref _last, exception);
        }

        /// <summary>
        /// Clears the captured exception and any NLog memory-target log
        /// lines. Called at the start of each scenario test's act phase.
        /// </summary>
        public static void Clear()
        {
            Volatile.Write(ref _last, null);
            lock (_gate)
            {
                _memoryTarget?.Logs.Clear();
            }
        }
    }
}
