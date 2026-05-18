using System;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.Core.ShellBuilders;
using ProjectV.Tests.Shared.Helpers.Mocks.Core;
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
    public sealed class ShellBuilderDirectorTests
    {
        public ShellBuilderDirectorTests()
        {
        }

        [Fact]
        public void Constructor_WithNullShellBuilder_ThrowsArgumentNullException()
        {
            // Act. / Assert.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var act = () => new ShellBuilderDirector(shellBuilder: null);
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("shellBuilder");
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void Constructor_WithValidShellBuilder_DoesNotThrow()
        {
            // Arrange.
            var shellBuilder = Substitute.For<IShellBuilder>();

            // Act.
            var act = () => new ShellBuilderDirector(shellBuilder);

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void ChangeShellBuilder_WithNull_ThrowsArgumentNullException()
        {
            // Arrange.
            var shellBuilder = Substitute.For<IShellBuilder>();
            var director = new ShellBuilderDirector(shellBuilder);

            // Act. / Assert.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var act = () => director.ChangeShellBuilder(newBuilder: null);
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("newBuilder");
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void MakeShell_InvokesEveryBuilderStep()
        {
            // Arrange.
            var shellBuilder = Substitute.For<IShellBuilder>();
            var expectedShell = CreateRealEmptyShell();
            shellBuilder.GetResult().Returns(expectedShell);
            var director = new ShellBuilderDirector(shellBuilder);

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
            var shellBuilder = Substitute.For<IShellBuilder>();
            var expectedShell = CreateRealEmptyShell();
            shellBuilder.GetResult().Returns(expectedShell);
            var director = new ShellBuilderDirector(shellBuilder);

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
            var originalBuilder = Substitute.For<IShellBuilder>();
            var replacementBuilder = Substitute.For<IShellBuilder>();
            var expectedShell = CreateRealEmptyShell();
            replacementBuilder.GetResult().Returns(expectedShell);

            var director = new ShellBuilderDirector(originalBuilder);

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
