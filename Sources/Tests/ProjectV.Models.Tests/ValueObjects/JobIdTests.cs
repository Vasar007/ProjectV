using System;
using AwesomeAssertions;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.Models.Tests.ValueObjects
{
    /// <summary>
    /// Unit tests for the <see cref="JobId" /> value-object — exercises
    /// <c>Create</c>, <c>Wrap</c>, <c>Parse</c>, <c>TryParse</c>, <c>None</c>,
    /// and <c>IsSpecified</c>. Uses <see cref="JobIdGenerator" /> for raw
    /// inputs.
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class JobIdTests
    {
        private readonly JobIdGenerator _generator;

        public JobIdTests()
        {
            _generator = new JobIdGenerator();
        }

        [Fact]
        public void NoneIsEqualToDefault()
        {
            // Arrange.
            JobId @default = default;

            // Act.
            JobId none = JobId.None;

            // Assert.
            none.Should().Be(@default);
            none.Value.Should().Be(Guid.Empty);
        }

        [Fact]
        public void NoneIsSpecifiedReturnsFalse()
        {
            // Arrange. / Act.
            bool isSpecified = JobId.None.IsSpecified;

            // Assert.
            isSpecified.Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsSpecifiedNonEmptyId()
        {
            // Arrange. / Act.
            var jobId = JobId.Create();

            // Assert.
            jobId.IsSpecified.Should().BeTrue();
            jobId.Value.Should().NotBe(Guid.Empty);
            jobId.Should().NotBe(JobId.None);
        }

        [Fact]
        public void CreateReturnsDistinctIdsOnEachCall()
        {
            // Arrange. / Act.
            var first = JobId.Create();
            var second = JobId.Create();

            // Assert.
            first.Should().NotBe(second);
        }

        [Fact]
        public void WrapWithNonEmptyGuidReturnsSpecifiedId()
        {
            // Arrange.
            var raw = Guid.NewGuid();

            // Act.
            var jobId = JobId.Wrap(raw);

            // Assert.
            jobId.Value.Should().Be(raw);
            jobId.IsSpecified.Should().BeTrue();
        }

        [Fact]
        public void WrapWithEmptyGuidThrowsArgumentException()
        {
            // Arrange.
            var act = () => JobId.Wrap(Guid.Empty);

            // Act. / Assert.
            act.Should().Throw<ArgumentException>()
               .WithParameterName("id");
        }

        [Fact]
        public void ParseRoundTripsThroughGenerator()
        {
            // Arrange.
            JobId expected = _generator.GenerateJobId();
            string raw = expected.Value.ToString();

            // Act.
            var actual = JobId.Parse(raw);

            // Assert.
            actual.Should().Be(expected);
            actual.IsSpecified.Should().BeTrue();
        }

        [Fact]
        public void ParseThrowsOnEmptyString()
        {
            // Arrange.
            var act = () => JobId.Parse(string.Empty);

            // Act. / Assert.
            act.Should().Throw<ArgumentException>()
               .WithParameterName("rawId");
        }

        [Fact]
        public void ParseThrowsOnNullString()
        {
            // Arrange.
            var act = () =>
            {
                _ = JobId.Parse(null!);
            };

            // Act. / Assert.
            act.Should().Throw<ArgumentException>()
               .WithParameterName("rawId");
        }

        [Fact]
        public void TryParseValidGuidReturnsTrueAndPopulatesResult()
        {
            // Arrange.
            string raw = _generator.GenerateRawId();

            // Act.
            bool success = JobId.TryParse(raw, out JobId result);

            // Assert.
            success.Should().BeTrue();
            result.IsSpecified.Should().BeTrue();
            result.Value.Should().Be(Guid.Parse(raw));
        }

        [Fact]
        public void TryParseInvalidStringReturnsFalseAndDefault()
        {
            // Arrange. / Act.
            bool success = JobId.TryParse("not-a-guid", out JobId result);

            // Assert.
            success.Should().BeFalse();
            result.Should().Be(default(JobId));
            result.IsSpecified.Should().BeFalse();
        }

        [Fact]
        public void TryParseNullReturnsFalseAndDefault()
        {
            // Arrange. / Act.
            bool success = JobId.TryParse(null, out JobId result);

            // Assert.
            success.Should().BeFalse();
            result.Should().Be(default(JobId));
        }

        [Fact]
        public void GeneratorCreateJobIdRoundTripsExplicitRaw()
        {
            // Arrange.
            string raw = _generator.GenerateRawId();

            // Act.
            JobId jobId = _generator.CreateJobId(raw);

            // Assert.
            jobId.IsSpecified.Should().BeTrue();
            jobId.Value.Should().Be(Guid.Parse(raw));
        }
    }
}
