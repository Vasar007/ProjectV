using System;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ProjectV.TelegramBotWebService.Tests.Scenarios.Helpers
{
    /// <summary>
    /// Diagnostic helper — reads back exception details logged by the
    /// production request pipeline so failing scenario tests can include
    /// the full stack in the AwesomeAssertions reason string. The
    /// production <c>ExceptionMiddleware</c> swallows the exception
    /// details into a generic HTTP 500 body and only logs the stack via
    /// NLog, so we attach a <see cref="MemoryTarget" /> to NLog to read
    /// those logs back from the test.
    /// </summary>
    /// <remarks>
    /// Used by the webhook and polling scenario base tests. The NLog
    /// memory target is process-global, so the capture is best-effort:
    /// when test classes run in parallel, log lines from another class
    /// may appear in the snapshot, and a <see cref="Clear" /> issued by
    /// one class can drop lines another class would have reported. That
    /// is acceptable because the captured lines feed assertion failure
    /// messages only — they are never asserted on directly.
    /// </remarks>
    internal static class CapturedException
    {
        private static readonly object _gate = new object();
        private static MemoryTarget? _memoryTarget;

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
        /// Clears any NLog memory-target log lines captured so far.
        /// Called at the start of each scenario test's act phase to keep
        /// the snapshot focused on the current test (best-effort under
        /// parallel test-class execution).
        /// </summary>
        public static void Clear()
        {
            lock (_gate)
            {
                _memoryTarget?.Logs.Clear();
            }
        }
    }
}
