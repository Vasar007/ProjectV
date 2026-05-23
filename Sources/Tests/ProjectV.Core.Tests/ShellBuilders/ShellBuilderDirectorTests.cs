using System;
using AutoFixture;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.Core.ShellBuilders;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Stubs.Core;
using Xunit;

namespace ProjectV.Core.Tests.ShellBuilders
{
    /// <summary>
    /// Unit tests for the <see cref="ShellBuilderDirector" /> orchestrator.
    /// </summary>
    /// <remarks>
    /// The director coordinates the GoF Builder pattern: it invokes
    /// <see cref="IShellBuilder.Reset" /> first, then the five
    /// <c>Build*</c> steps in declared order, and finally
    /// <see cref="IShellBuilder.GetResult" />. These tests verify that
    /// contract via an NSubstitute substitute of <see cref="IShellBuilder" />.
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class ShellBuilderDirectorTests : BaseMockTest
    {
        public ShellBuilderDirectorTests()
        {
        }

        [Fact]
        public void Constructor_WithNullShellBuilder_ThrowsArgumentNullException()
        {
            // Act. / Assert.
            var act = () => new ShellBuilderDirector(shellBuilder: null!);
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("shellBuilder");
        }

        [Fact]
        public void Constructor_WithValidShellBuilder_DoesNotThrow()
        {
            // Arrange.
            var shellBuilder = Fixture.Create<IShellBuilder>();

            // Act.
            var act = () => new ShellBuilderDirector(shellBuilder);

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void ChangeShellBuilder_WithNull_ThrowsArgumentNullException()
        {
            // Arrange.
            var shellBuilder = Fixture.Create<IShellBuilder>();
            var director = BuildSut(shellBuilder);

            // Act. / Assert.
            var act = () => director.ChangeShellBuilder(newBuilder: null!);
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("newBuilder");
        }

        [Fact]
        public void MakeShell_InvokesEveryBuilderStep()
        {
            // Arrange.
            var shellBuilder = Fixture.Create<IShellBuilder>();
            var expectedShell = CreateRealEmptyShell();
            shellBuilder.GetResult().Returns(expectedShell);
            var director = BuildSut(shellBuilder);

            // Act.
            Shell actualValue = director.MakeShell();

            // Assert.
            actualValue.Should().BeSameAs(expectedShell);
            shellBuilder.Received(1).Reset();
            shellBuilder.Received(1).BuildMessageHandler();
            shellBuilder.Received(1).BuildInputManager();
            shellBuilder.Received(1).BuildCrawlersManager();
            shellBuilder.Received(1).BuildAppraisersManager();
            shellBuilder.Received(1).BuildOutputManager();
            shellBuilder.Received(1).GetResult();

            // Cleanup local Shell — Director returns ownership to caller.
            expectedShell.Dispose();
        }

        [Fact]
        public void MakeShell_InvokesBuilderStepsInDeclaredOrder()
        {
            // Arrange.
            var shellBuilder = Fixture.Create<IShellBuilder>();
            var expectedShell = CreateRealEmptyShell();
            shellBuilder.GetResult().Returns(expectedShell);
            var director = BuildSut(shellBuilder);

            // Act.
            director.MakeShell();

            // Assert.
            Received.InOrder(() =>
            {
                shellBuilder.Reset();
                shellBuilder.BuildMessageHandler();
                shellBuilder.BuildInputManager();
                shellBuilder.BuildCrawlersManager();
                shellBuilder.BuildAppraisersManager();
                shellBuilder.BuildOutputManager();
                shellBuilder.GetResult();
            });

            expectedShell.Dispose();
        }

        [Fact]
        public void MakeShell_AfterChangeShellBuilder_DispatchesToReplacedBuilder()
        {
            // Arrange.
            var originalBuilder = Fixture.Create<IShellBuilder>();
            var replacementBuilder = Fixture.Create<IShellBuilder>();
            var expectedShell = CreateRealEmptyShell();
            replacementBuilder.GetResult().Returns(expectedShell);

            var director = BuildSut(originalBuilder);

            // Act.
            director.ChangeShellBuilder(replacementBuilder);
            originalBuilder.ClearReceivedCalls();
            director.MakeShell();

            // Assert.
            originalBuilder.DidNotReceive().Reset();
            replacementBuilder.Received(1).Reset();
            replacementBuilder.Received(1).GetResult();

            expectedShell.Dispose();
        }

        /// <summary>
        /// Builds the <see cref="ShellBuilderDirector" /> SUT from the
        /// supplied <see cref="IShellBuilder" /> collaborator. Per-test
        /// builder helper that mirrors the production constructor.
        /// </summary>
        private static ShellBuilderDirector BuildSut(IShellBuilder shellBuilder)
        {
            return new ShellBuilderDirector(shellBuilder);
        }

        /// <summary>
        /// Creates a real empty <see cref="Shell" /> instance via the
        /// shared <c>TestShellBuilder</c> for use as the return value of
        /// the substituted <see cref="IShellBuilder.GetResult" /> method.
        /// </summary>
        private static Shell CreateRealEmptyShell()
        {
            return TestShellBuilder.CreateWithoutSetup();
        }
    }
}
