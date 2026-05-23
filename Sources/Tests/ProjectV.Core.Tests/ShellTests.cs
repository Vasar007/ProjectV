using System;
using System.Xml.Linq;
using AwesomeAssertions;
using ProjectV.Appraisers;
using ProjectV.Core.ShellBuilders;
using ProjectV.Crawlers;
using ProjectV.IO.Input;
using ProjectV.IO.Output;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Stubs.Appraisers;
using ProjectV.Tests.Shared.Helpers.Stubs.Core;
using ProjectV.Tests.Shared.Helpers.Stubs.Managers;
using Xunit;

namespace ProjectV.Core.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="Shell" /> orchestration entry point.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="Shell" /> takes concrete-typed (sealed) managers
    /// (<see cref="InputManager" />, <see cref="CrawlersManager" />,
    /// <see cref="AppraisersManager" />, <see cref="OutputManager" />) —
    /// a known architectural anti-pattern in this codebase. Tests work
    /// AROUND that coupling via real manager instances populated with
    /// NSubstitute children (<see cref="TestShellBuilder" /> + the manager
    /// builders); they do NOT refactor <see cref="Shell" />.
    /// </para>
    /// <para>
    /// Coverage scope for this Unit suite is intentionally narrow:
    /// constructor null-guards, property surface, <see cref="Shell.Dispose" />
    /// idempotency, and the <see cref="Shell.CreateBuilderDirector" />
    /// static factory. The <c>Run</c> success / error / output-error
    /// branches are NOT exercised here because the Gridsum.DataflowEx
    /// pipeline that <c>Run</c> drives requires a fully-composed pipeline
    /// (at least one inputter, crawler, and appraiser per stage) to
    /// terminate deterministically — that scenario belongs in an
    /// integration test plan (Phase 3 E2E or the JWT integration plan).
    /// </para>
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class ShellTests : BaseMockTest
    {
        public ShellTests()
        {
        }

        [Fact]
        public void Constructor_WithValidManagers_PopulatesAllProperties()
        {
            // Arrange.
            var inputManager = TestInputManagerBuilder.CreateWithoutSetup();
            var crawlersManager = TestCrawlersManagerBuilder.CreateWithoutSetup();
            var appraisersManager = TestAppraisersManagerBuilder.CreateWithoutSetup();
            var outputManager = TestOutputManagerBuilder.CreateWithoutSetup();

            // Act.
            using var shell = new Shell(
                inputManager, crawlersManager, appraisersManager, outputManager,
                boundedCapacity: 10
            );

            // Assert.
            shell.InputManager.Should().BeSameAs(inputManager);
            shell.CrawlersManager.Should().BeSameAs(crawlersManager);
            shell.AppraisersManager.Should().BeSameAs(appraisersManager);
            shell.OutputManager.Should().BeSameAs(outputManager);
        }

        [Fact]
        public void Constructor_WithNullInputManager_ThrowsArgumentNullException()
        {
            // Arrange.
            var crawlersManager = TestCrawlersManagerBuilder.CreateWithoutSetup();
            var appraisersManager = TestAppraisersManagerBuilder.CreateWithoutSetup();
            var outputManager = TestOutputManagerBuilder.CreateWithoutSetup();

            // Act. / Assert.
            var act = () => new Shell(
                inputManager: null!,
                crawlersManager, appraisersManager, outputManager,
                boundedCapacity: 10
            );
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("inputManager");
        }

        [Fact]
        public void Constructor_WithNullCrawlersManager_ThrowsArgumentNullException()
        {
            // Arrange.
            var inputManager = TestInputManagerBuilder.CreateWithoutSetup();
            var appraisersManager = TestAppraisersManagerBuilder.CreateWithoutSetup();
            var outputManager = TestOutputManagerBuilder.CreateWithoutSetup();

            // Act. / Assert.
            var act = () => new Shell(
                inputManager,
                crawlersManager: null!,
                appraisersManager, outputManager,
                boundedCapacity: 10
            );
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("crawlersManager");
        }

        [Fact]
        public void Constructor_WithNullAppraisersManager_ThrowsArgumentNullException()
        {
            // Arrange.
            var inputManager = TestInputManagerBuilder.CreateWithoutSetup();
            var crawlersManager = TestCrawlersManagerBuilder.CreateWithoutSetup();
            var outputManager = TestOutputManagerBuilder.CreateWithoutSetup();

            // Act. / Assert.
            var act = () => new Shell(
                inputManager, crawlersManager,
                appraisersManager: null!,
                outputManager,
                boundedCapacity: 10
            );
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("appraisersManager");
        }

        [Fact]
        public void Constructor_WithNullOutputManager_ThrowsArgumentNullException()
        {
            // Arrange.
            var inputManager = TestInputManagerBuilder.CreateWithoutSetup();
            var crawlersManager = TestCrawlersManagerBuilder.CreateWithoutSetup();
            var appraisersManager = TestAppraisersManagerBuilder.CreateWithoutSetup();

            // Act. / Assert.
            var act = () => new Shell(
                inputManager, crawlersManager, appraisersManager,
                outputManager: null!,
                boundedCapacity: 10
            );
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("outputManager");
        }

        [Fact]
        public void Dispose_CalledTwice_IsIdempotent()
        {
            // Arrange.
            var shell = TestShellBuilder.CreateWithoutSetup();

            // Act.
            shell.Dispose();
            var act = () => shell.Dispose();

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void CreateBuilderDirector_WithMinimalValidXDocument_ReturnsNonNullDirector()
        {
            // Arrange.
            var configuration = CreateMinimalShellConfigXml();

            // Act.
            ShellBuilderDirector director = Shell.CreateBuilderDirector(configuration);

            // Assert.
            director.Should().NotBeNull();
        }

        /// <summary>
        /// Builds a minimal valid <see cref="XDocument" /> that satisfies the
        /// <see cref="ShellBuilderFromXDocument" /> constructor (only the
        /// <c>ShellConfig</c> root element is required at construction time;
        /// individual sub-elements are only parsed lazily during the
        /// <c>Build*Manager</c> steps).
        /// </summary>
        private static XDocument CreateMinimalShellConfigXml()
        {
            return new XDocument(
                new XElement("Root",
                    new XElement("ShellConfig")
                )
            );
        }
    }
}
