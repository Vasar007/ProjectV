using Acolyte.Assertions;
using ProjectV.Appraisers;
using ProjectV.Core;
using ProjectV.Crawlers;
using ProjectV.IO.Input;
using ProjectV.IO.Output;
using ProjectV.Tests.Shared.Helpers.Stubs.Appraisers;
using ProjectV.Tests.Shared.Helpers.Stubs.Managers;

namespace ProjectV.Tests.Shared.Helpers.Stubs.Core
{
    /// <summary>
    /// Builder for real <see cref="Shell" /> instances composed from the four
    /// production manager types (<see cref="InputManager" />,
    /// <see cref="CrawlersManager" />, <see cref="AppraisersManager" />,
    /// <see cref="OutputManager" />) populated with
    /// <see cref="NSubstitute" /> child doubles (Decision D-33 fallback).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Shell" /> takes concrete-typed managers, not interfaces
    /// (an architectural anti-pattern documented in
    /// <c>.planning/codebase/ARCHITECTURE.md</c>); this builder works
    /// around the coupling by composing real managers populated with
    /// substituted children via the sibling
    /// <see cref="TestInputManagerBuilder" />,
    /// <see cref="TestCrawlersManagerBuilder" />,
    /// <see cref="TestAppraisersManagerBuilder" />, and
    /// <see cref="TestOutputManagerBuilder" /> classes.
    /// </para>
    /// <para>
    /// The plan does not refactor <see cref="Shell" /> — the manager-typed
    /// constructor parameters stay as production declares them.
    /// </para>
    /// </remarks>
    public sealed class TestShellBuilder
    {
        /// <summary>
        /// Default bounded capacity for the resulting <see cref="Shell" />.
        /// </summary>
        public const int DefaultBoundedCapacity = 10;

        private InputManager? _inputManager;
        private CrawlersManager? _crawlersManager;
        private AppraisersManager? _appraisersManager;
        private OutputManager? _outputManager;
        private int _boundedCapacity = DefaultBoundedCapacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestShellBuilder" />
        /// class. The four manager slots are initially unset; the
        /// <see cref="Build" /> method fills any unset slot with the empty
        /// builder default (<c>CreateWithoutSetup()</c>).
        /// </summary>
        public TestShellBuilder()
        {
        }

        /// <summary>
        /// Convenience factory that returns a <see cref="Shell" /> composed
        /// from four empty managers (no inputters, crawlers, appraisers, or
        /// outputters registered) and the default bounded capacity.
        /// </summary>
        public static Shell CreateWithoutSetup()
        {
            return new TestShellBuilder().Build();
        }

        /// <summary>
        /// Overrides the <see cref="InputManager" /> slot.
        /// </summary>
        /// <param name="inputManager">
        /// Pre-built manager to plug into the resulting <see cref="Shell" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestShellBuilder WithInputManager(InputManager inputManager)
        {
            _inputManager = inputManager.ThrowIfNull(nameof(inputManager));
            return this;
        }

        /// <summary>
        /// Overrides the <see cref="CrawlersManager" /> slot.
        /// </summary>
        /// <param name="crawlersManager">
        /// Pre-built manager to plug into the resulting <see cref="Shell" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestShellBuilder WithCrawlersManager(CrawlersManager crawlersManager)
        {
            _crawlersManager = crawlersManager.ThrowIfNull(nameof(crawlersManager));
            return this;
        }

        /// <summary>
        /// Overrides the <see cref="AppraisersManager" /> slot.
        /// </summary>
        /// <param name="appraisersManager">
        /// Pre-built manager to plug into the resulting <see cref="Shell" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestShellBuilder WithAppraisersManager(AppraisersManager appraisersManager)
        {
            _appraisersManager = appraisersManager.ThrowIfNull(nameof(appraisersManager));
            return this;
        }

        /// <summary>
        /// Overrides the <see cref="OutputManager" /> slot.
        /// </summary>
        /// <param name="outputManager">
        /// Pre-built manager to plug into the resulting <see cref="Shell" />.
        /// Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestShellBuilder WithOutputManager(OutputManager outputManager)
        {
            _outputManager = outputManager.ThrowIfNull(nameof(outputManager));
            return this;
        }

        /// <summary>
        /// Overrides the bounded capacity passed to the
        /// <see cref="Shell" /> constructor.
        /// </summary>
        /// <param name="boundedCapacity">Bounded capacity value.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestShellBuilder WithBoundedCapacity(int boundedCapacity)
        {
            _boundedCapacity = boundedCapacity;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="Shell" /> instance. Any manager slot that
        /// has not been explicitly set is filled with the corresponding
        /// builder's <c>CreateWithoutSetup()</c> default.
        /// </summary>
        public Shell Build()
        {
            var inputManager = _inputManager ?? TestInputManagerBuilder.CreateWithoutSetup();
            var crawlersManager = _crawlersManager ?? TestCrawlersManagerBuilder.CreateWithoutSetup();
            var appraisersManager =
                _appraisersManager ?? TestAppraisersManagerBuilder.CreateWithoutSetup();
            var outputManager = _outputManager ?? TestOutputManagerBuilder.CreateWithoutSetup();

            return new Shell(
                inputManager,
                crawlersManager,
                appraisersManager,
                outputManager,
                _boundedCapacity
            );
        }
    }
}
