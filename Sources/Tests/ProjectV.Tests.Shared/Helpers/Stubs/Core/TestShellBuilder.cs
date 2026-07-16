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
    /// <c>NSubstitute</c> child doubles.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Shell" /> takes concrete-typed managers, not interfaces
    /// (a known architectural anti-pattern in this codebase); this builder
    /// works around the coupling by composing real managers populated with
    /// substituted children via the sibling
    /// <see cref="TestInputManagerBuilder" />,
    /// <see cref="TestCrawlersManagerBuilder" />,
    /// <see cref="TestAppraisersManagerBuilder" />, and
    /// <see cref="TestOutputManagerBuilder" /> classes.
    /// </para>
    /// <para>
    /// The builder does not abstract <see cref="Shell" /> away — the
    /// manager-typed constructor parameters stay as production declares them.
    /// </para>
    /// </remarks>
    public sealed class TestShellBuilder
    {
        /// <summary>
        /// Default bounded capacity for the resulting <see cref="Shell" />.
        /// </summary>
        public const int DefaultBoundedCapacity = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestShellBuilder" />
        /// class. The <see cref="Build" /> method composes the
        /// <see cref="Shell" /> from the four sibling builders' empty
        /// defaults (<c>CreateWithoutSetup()</c>).
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
        /// Builds the <see cref="Shell" /> instance. Every manager is the
        /// corresponding sibling builder's <c>CreateWithoutSetup()</c>
        /// default.
        /// </summary>
        public Shell Build()
        {
            InputManager inputManager = TestInputManagerBuilder.CreateWithoutSetup();
            CrawlersManager crawlersManager = TestCrawlersManagerBuilder.CreateWithoutSetup();
            AppraisersManager appraisersManager = TestAppraisersManagerBuilder.CreateWithoutSetup();
            OutputManager outputManager = TestOutputManagerBuilder.CreateWithoutSetup();

            return new Shell(
                inputManager,
                crawlersManager,
                appraisersManager,
                outputManager,
                DefaultBoundedCapacity
            );
        }
    }
}
