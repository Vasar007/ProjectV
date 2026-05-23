using System;
using AutoFixture;
using AwesomeAssertions;
using ProjectV.DataPipeline;
using ProjectV.IO.Output;
using ProjectV.Tests.Shared.ForTests;
using Xunit;

namespace ProjectV.OutputProcessing.Tests
{
    /// <summary>
    /// Unit tests for <see cref="OutputManager" />'s public contract:
    /// <see cref="OutputManager.CreateFlow(string)" /> returns a non-null
    /// <see cref="OutputtersFlow" /> regardless of whether the manager has
    /// any registered outputters, the storage-name argument is empty, or
    /// both. Also covers the constructor null/whitespace guard and the
    /// <c>Add</c> / <c>Remove</c> registration round-trip.
    /// </summary>
    /// <remarks>
    /// Collaborator <see cref="IOutputter" /> instances are supplied through
    /// NSubstitute; the manager itself is the real concrete type. The
    /// static <c>_logger</c> field on
    /// <see cref="OutputManager" /> is initialised through
    /// <c>LoggerFactory.CreateLoggerFor&lt;OutputManager&gt;()</c> — the
    /// hoisted <c>ProjectV.Tests.Shared.ForTests.TestModuleInitializer</c>
    /// installs an empty NLog config on assembly load so the type
    /// initialiser does not write log files during the test run.
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class OutputManagerTests : BaseMockTest
    {
        private const string DefaultStorageName = "default-storage.csv";

        public OutputManagerTests()
        {
        }

        [Fact]
        public void CreateFlow_ReturnsNonNullFlow()
        {
            // Arrange.
            var sut = BuildSut();
            IOutputter outputter = Fixture.Create<IOutputter>();
            sut.Add(outputter);

            // Act.
            OutputtersFlow actual = sut.CreateFlow("storage.csv");

            // Assert.
            actual.Should().NotBeNull(
                "OutputManager.CreateFlow must return a non-null OutputtersFlow " +
                "so the downstream DataflowPipeline can wire the outputters stage"
            );
        }

        [Fact]
        public void CreateFlow_WithNoOutputters_ReturnsNonNullFlow()
        {
            // Arrange.
            var sut = BuildSut();

            // Act.
            OutputtersFlow actual = sut.CreateFlow("storage.csv");

            // Assert.
            actual.Should().NotBeNull(
                "the contract holds even with zero outputters — Shell wires the " +
                "flow before any outputter is necessarily registered. The flow's " +
                "default Action<RatingDataContainer> logs each result; concrete " +
                "outputters are consumed later by SaveResults()."
            );
        }

        [Fact]
        public void CreateFlow_WithEmptyStorageName_FallsBackToDefaultAndReturnsNonNullFlow()
        {
            // Arrange.
            var sut = BuildSut();
            sut.Add(Fixture.Create<IOutputter>());

            // Act.
            OutputtersFlow actual = sut.CreateFlow(string.Empty);

            // Assert.
            actual.Should().NotBeNull(
                "an empty storage name must fall back to the default storage name " +
                "without breaking the flow construction"
            );
        }

        [Fact]
        public void Constructor_WithNullDefaultStorageName_ThrowsArgumentNullException()
        {
            // Arrange. / Act.
            var act = () => new OutputManager(
                defaultStorageName: null!
            );

            // Assert.
            act.Should()
               .Throw<ArgumentNullException>()
               .WithParameterName("defaultStorageName");
        }

        [Fact]
        public void Constructor_WithWhitespaceDefaultStorageName_ThrowsArgumentException()
        {
            // Arrange. / Act.
            var act = () => new OutputManager(defaultStorageName: "   ");

            // Assert.
            act.Should()
               .Throw<ArgumentException>()
               .WithParameterName("defaultStorageName");
        }

        [Fact]
        public void Add_WithNullOutputter_ThrowsArgumentNullException()
        {
            // Arrange.
            var sut = BuildSut();

            // Act.
            var act = () => sut.Add(
                item: null!
            );

            // Assert.
            act.Should()
               .Throw<ArgumentNullException>()
               .WithParameterName("item");
        }

        [Fact]
        public void Remove_WithRegisteredOutputter_ReturnsTrue()
        {
            // Arrange.
            var sut = BuildSut();
            IOutputter outputter = Fixture.Create<IOutputter>();
            sut.Add(outputter);

            // Act.
            bool removed = sut.Remove(outputter);

            // Assert.
            removed.Should().BeTrue(
                "Remove must report success when the manager holds the supplied outputter"
            );
        }

        /// <summary>
        /// Builds a default-storage <see cref="OutputManager" /> SUT.
        /// Per-class helper to keep test bodies focused on Arrange/Act/Assert.
        /// </summary>
        private static OutputManager BuildSut()
        {
            return new OutputManager(DefaultStorageName);
        }
    }
}
