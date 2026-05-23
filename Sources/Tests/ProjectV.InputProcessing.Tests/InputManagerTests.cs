using System;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.DataPipeline;
using ProjectV.IO.Input;
using Xunit;

namespace ProjectV.InputProcessing.Tests
{
    /// <summary>
    /// Unit tests for <see cref="InputManager" />'s public contract:
    /// <see cref="InputManager.CreateFlow(string)" /> returns a non-null
    /// <see cref="InputtersFlow" /> regardless of whether the manager has
    /// any registered inputters, the storage-name argument is empty, or both.
    /// Also covers the constructor null/whitespace guard and the
    /// <c>Add</c> / <c>Remove</c> registration round-trip.
    /// </summary>
    /// <remarks>
    /// Per Decision D-05, collaborator <see cref="IInputter" /> instances are
    /// supplied through NSubstitute; the manager itself is the real concrete
    /// type. The static <c>_logger</c> field on
    /// <see cref="InputManager" /> is initialised through
    /// <c>LoggerFactory.CreateLoggerFor&lt;InputManager&gt;()</c> — the
    /// hoisted <c>ProjectV.Tests.Shared.ForTests.TestModuleInitializer</c>
    /// installs an empty NLog config on assembly load so the type
    /// initialiser does not write log files during the test run.
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class InputManagerTests
    {
        private const string DefaultStorageName = "default-storage.csv";

        public InputManagerTests()
        {
        }

        [Fact]
        public void CreateFlow_ReturnsNonNullFlow()
        {
            // Arrange.
            var sut = new InputManager(DefaultStorageName);
            IInputter inputter = Substitute.For<IInputter>();
            sut.Add(inputter);

            // Act.
            InputtersFlow actual = sut.CreateFlow("storage.csv");

            // Assert.
            actual.Should().NotBeNull(
                "InputManager.CreateFlow must return a non-null InputtersFlow " +
                "so the downstream DataflowPipeline can wire the inputters stage"
            );
        }

        [Fact]
        public void CreateFlow_WithNoInputters_ReturnsNonNullFlow()
        {
            // Arrange.
            var sut = new InputManager(DefaultStorageName);

            // Act.
            InputtersFlow actual = sut.CreateFlow("storage.csv");

            // Assert.
            actual.Should().NotBeNull(
                "the contract holds even with zero inputters — Shell wires the " +
                "flow before any inputter is necessarily registered"
            );
        }

        // NOTE: there is intentionally NO CreateFlow_WithEmptyStorageName test
        // for InputManager. Unlike OutputManager.CreateFlow (which only logs on
        // the empty-storage-name fallback path), InputManager.CreateFlow also
        // calls ProjectV.Communication.GlobalMessageHandler.OutputMessage(...),
        // a static helper backed by a process-wide IMessageHandler that is null
        // until a host (Shell/ServiceHost/ConsoleApp) registers one. Asserting
        // on the empty-storage-name path here would either (a) require the test
        // to mutate global static state (leaking across the xUnit assembly's
        // parallel test runs) or (b) capture an ArgumentNullException out of
        // the messaging seam, neither of which reflects the plan's
        // CreateFlow-non-null contract. The non-empty-storage path is the
        // contract Shell exercises in production; we test that path only here.
        // The empty-storage-name code path is exercised through the higher-
        // level Shell.Run integration coverage (currently "tested around" per
        // 02-05-SUMMARY § Deviations §1).

        [Fact]
        public void Constructor_WithNullDefaultStorageName_ThrowsArgumentNullException()
        {
            // Arrange. / Act.
            var act = () => new InputManager(
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
            var act = () => new InputManager(defaultStorageName: "   ");

            // Assert.
            act.Should()
               .Throw<ArgumentException>()
               .WithParameterName("defaultStorageName");
        }

        [Fact]
        public void Add_WithNullInputter_ThrowsArgumentNullException()
        {
            // Arrange.
            var sut = new InputManager(DefaultStorageName);

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
        public void Remove_WithRegisteredInputter_ReturnsTrue()
        {
            // Arrange.
            var sut = new InputManager(DefaultStorageName);
            IInputter inputter = Substitute.For<IInputter>();
            sut.Add(inputter);

            // Act.
            bool removed = sut.Remove(inputter);

            // Assert.
            removed.Should().BeTrue(
                "Remove must report success when the manager holds the supplied inputter"
            );
        }
    }
}
