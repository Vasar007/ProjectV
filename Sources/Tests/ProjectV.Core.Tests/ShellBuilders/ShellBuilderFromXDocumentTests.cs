using System;
using System.Xml.Linq;
using AwesomeAssertions;
using ProjectV.Core.ShellBuilders;
using Xunit;

namespace ProjectV.Core.Tests.ShellBuilders
{
    /// <summary>
    /// Unit tests for the <see cref="ShellBuilderFromXDocument" /> XML
    /// configuration parser. Focuses on the constructor null/root-null
    /// guards and the <c>Reset</c> + <c>GetResult</c> contracts that do not
    /// require a fully-populated XML config (the builder's individual
    /// <c>Build*Manager</c> steps are exercised indirectly via the
    /// <see cref="ShellBuilderDirector" /> test suite and the production
    /// integration path).
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class ShellBuilderFromXDocumentTests
    {
        public ShellBuilderFromXDocumentTests()
        {
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ThrowsArgumentNullException()
        {
            // Act. / Assert.
            var act = () => new ShellBuilderFromXDocument(configuration: null!);
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("configuration");
        }

        [Fact]
        public void Constructor_WithMissingRoot_ThrowsArgumentNullException()
        {
            // Arrange.
            var configuration = new XDocument();

            // Act. / Assert.
            var act = () => new ShellBuilderFromXDocument(configuration);
            act.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("Root");
        }

        [Fact]
        public void Constructor_WithMinimalValidConfiguration_DoesNotThrow()
        {
            // Arrange.
            var configuration = new XDocument(
                new XElement("Root",
                    new XElement("ShellConfig")
                )
            );

            // Act.
            var act = () => new ShellBuilderFromXDocument(configuration);

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void GetResult_BeforeAnyBuildStep_ThrowsInvalidOperationException()
        {
            // Arrange.
            var configuration = new XDocument(
                new XElement("Root",
                    new XElement("ShellConfig")
                )
            );
            var builder = new ShellBuilderFromXDocument(configuration);

            // Act. / Assert.
            // GetResult() validates that all four manager slots are populated;
            // since no Build*Manager step has been called yet, the first slot
            // check (InputManager) trips the guard.
            var act = () => builder.GetResult();
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("*InputManager*not built*");
        }

        [Fact]
        public void Reset_AfterCtor_DoesNotThrow()
        {
            // Arrange.
            var configuration = new XDocument(
                new XElement("Root",
                    new XElement("ShellConfig")
                )
            );
            var builder = new ShellBuilderFromXDocument(configuration);

            // Act.
            var act = () => builder.Reset();

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void BuildMessageHandler_WithMissingElement_ThrowsInvalidOperationException()
        {
            // Arrange.
            var configuration = new XDocument(
                new XElement("Root",
                    new XElement("ShellConfig")
                )
            );
            var builder = new ShellBuilderFromXDocument(configuration);

            // Act. / Assert.
            // MessageHandler element is absent — production code throws
            // InvalidOperationException with the parameter name in the message.
            var act = () => builder.BuildMessageHandler();
            act.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("*MessageHandler*");
        }
    }
}
