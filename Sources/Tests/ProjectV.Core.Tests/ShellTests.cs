using System;
using System.Collections.Generic;
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
    /// AROUND that coupling via real (empty) manager instances
    /// (<see cref="TestShellBuilder" /> + the manager builders); they do
    /// NOT refactor <see cref="Shell" />.
    /// </para>
    /// <para>
    /// Coverage scope for this Unit suite is intentionally narrow:
    /// constructor null-guards, property surface, <see cref="Shell.Dispose" />
    /// idempotency, and the <see cref="Shell.CreateBuilderDirector" />
    /// static factory. The <c>Run</c> success / error / output-error
    /// branches are NOT exercised here because the Gridsum.DataflowEx
    /// pipeline that <c>Run</c> drives requires a fully-composed pipeline
    /// (at least one inputter, crawler, and appraiser per stage) to
    /// terminate deterministically — that scenario belongs in a future
    /// end-to-end integration suite.
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
            var inputManager = CreateInputManager();
            var crawlersManager = CreateCrawlersManager();
            var appraisersManager = CreateAppraisersManager();
            var outputManager = CreateOutputManager();

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
        public void Constructor_OnAllArgumentsProvided_CreatesInstance()
        {
            // Arrange.
            var inputManager = CreateInputManager();
            var crawlersManager = CreateCrawlersManager();
            var appraisersManager = CreateAppraisersManager();
            var outputManager = CreateOutputManager();

            // Act.
            var act = () => CreateShellWithAllArguments(
                inputManager: inputManager,
                crawlersManager: crawlersManager,
                appraisersManager: appraisersManager,
                outputManager: outputManager
            );

            // Assert.
            act.Should().NotThrow().Which.Dispose();
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenNullValueProvided()
        {
            // Arrange.
            var inputManager = CreateInputManager();
            var crawlersManager = CreateCrawlersManager();
            var appraisersManager = CreateAppraisersManager();
            var outputManager = CreateOutputManager();

            var actions = new List<(Action act, string paramName)>
            {
                (() => CreateShellWithAllArguments(
                    inputManager: null!,
                    crawlersManager: crawlersManager,
                    appraisersManager: appraisersManager,
                    outputManager: outputManager),
                    "inputManager"),
                (() => CreateShellWithAllArguments(
                    inputManager: inputManager,
                    crawlersManager: null!,
                    appraisersManager: appraisersManager,
                    outputManager: outputManager),
                    "crawlersManager"),
                (() => CreateShellWithAllArguments(
                    inputManager: inputManager,
                    crawlersManager: crawlersManager,
                    appraisersManager: null!,
                    outputManager: outputManager),
                    "appraisersManager"),
                (() => CreateShellWithAllArguments(
                    inputManager: inputManager,
                    crawlersManager: crawlersManager,
                    appraisersManager: appraisersManager,
                    outputManager: null!),
                    "outputManager"),
            };

            // Act. / Assert.
            foreach ((Action act, string paramName) in actions)
            {
                act.Should()
                    .Throw<ArgumentNullException>()
                    .WithParameterName(paramName);
            }
        }

        [Fact]
        public void Dispose_CalledTwice_IsIdempotent()
        {
            // Arrange.
            var shell = CreateShell();

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
        /// Creates a <see cref="Shell" /> from every constructor dependency
        /// explicitly. Constructor tests arrange valid managers via the
        /// per-dependency <c>Create*</c> helpers and pass exactly one
        /// dependency as <c>null!</c> to exercise the matching null-guard.
        /// </summary>
        private static Shell CreateShellWithAllArguments(
            InputManager inputManager,
            CrawlersManager crawlersManager,
            AppraisersManager appraisersManager,
            OutputManager outputManager)
        {
            return new Shell(
                inputManager: inputManager,
                crawlersManager: crawlersManager,
                appraisersManager: appraisersManager,
                outputManager: outputManager,
                boundedCapacity: 10
            );
        }

        /// <summary>
        /// Creates a default-configured <see cref="InputManager" /> via
        /// <see cref="TestInputManagerBuilder" />. Per-class helper so test
        /// bodies do not call builders directly.
        /// </summary>
        private static InputManager CreateInputManager()
        {
            return TestInputManagerBuilder.CreateWithoutSetup();
        }

        /// <summary>
        /// Creates a default-configured <see cref="CrawlersManager" /> via
        /// <see cref="TestCrawlersManagerBuilder" />. Per-class helper so test
        /// bodies do not call builders directly.
        /// </summary>
        private static CrawlersManager CreateCrawlersManager()
        {
            return TestCrawlersManagerBuilder.CreateWithoutSetup();
        }

        /// <summary>
        /// Creates a default-configured <see cref="AppraisersManager" /> via
        /// <see cref="TestAppraisersManagerBuilder" />. Per-class helper so test
        /// bodies do not call builders directly.
        /// </summary>
        private static AppraisersManager CreateAppraisersManager()
        {
            return TestAppraisersManagerBuilder.CreateWithoutSetup();
        }

        /// <summary>
        /// Creates a default-configured <see cref="OutputManager" /> via
        /// <see cref="TestOutputManagerBuilder" />. Per-class helper so test
        /// bodies do not call builders directly.
        /// </summary>
        private static OutputManager CreateOutputManager()
        {
            return TestOutputManagerBuilder.CreateWithoutSetup();
        }

        /// <summary>
        /// Creates a fully-composed <see cref="Shell" /> backed by empty
        /// default managers via <see cref="TestShellBuilder" />. Per-class
        /// helper so test bodies do not call builders directly.
        /// </summary>
        private static Shell CreateShell()
        {
            return TestShellBuilder.CreateWithoutSetup();
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
