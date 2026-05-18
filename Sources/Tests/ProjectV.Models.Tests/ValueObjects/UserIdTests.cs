using System;
using AwesomeAssertions;
using ProjectV.Models.Users;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.Models.Tests.ValueObjects
{
    /// <summary>
    /// Unit tests for the <see cref="UserId" /> value-object — exercises
    /// <c>Create</c>, <c>Wrap</c>, <c>Parse</c>, <c>TryParse</c>, <c>None</c>,
    /// and <c>IsSpecified</c>. Uses <see cref="UserIdGenerator" /> for raw
    /// inputs.
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class UserIdTests
    {
        private readonly UserIdGenerator _generator;

        public UserIdTests()
        {
            _generator = new UserIdGenerator();
        }

        [Fact]
        public void NoneIsEqualToDefault()
        {
            // Arrange.
            UserId @default = default;

            // Act.
            UserId none = UserId.None;

            // Assert.
            none.Should().Be(@default);
            none.Value.Should().Be(Guid.Empty);
        }

        [Fact]
        public void NoneIsSpecifiedReturnsFalse()
        {
            // Arrange. / Act.
            bool isSpecified = UserId.None.IsSpecified;

            // Assert.
            isSpecified.Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsSpecifiedNonEmptyId()
        {
            // Arrange. / Act.
            var userId = UserId.Create();

            // Assert.
            userId.IsSpecified.Should().BeTrue();
            userId.Value.Should().NotBe(Guid.Empty);
            userId.Should().NotBe(UserId.None);
        }

        [Fact]
        public void CreateReturnsDistinctIdsOnEachCall()
        {
            // Arrange. / Act.
            var first = UserId.Create();
            var second = UserId.Create();

            // Assert.
            first.Should().NotBe(second);
        }

        [Fact]
        public void WrapWithNonEmptyGuidReturnsSpecifiedId()
        {
            // Arrange.
            var raw = Guid.NewGuid();

            // Act.
            var userId = UserId.Wrap(raw);

            // Assert.
            userId.Value.Should().Be(raw);
            userId.IsSpecified.Should().BeTrue();
        }

        [Fact]
        public void WrapWithEmptyGuidThrowsArgumentException()
        {
            // Arrange.
            var act = () => UserId.Wrap(Guid.Empty);

            // Act. / Assert.
            act.Should().Throw<ArgumentException>()
               .WithParameterName("id");
        }

        [Fact]
        public void ParseRoundTripsThroughGenerator()
        {
            // Arrange.
            UserId expected = _generator.GenerateUserId();
            string raw = expected.Value.ToString();

            // Act.
            var actual = UserId.Parse(raw);

            // Assert.
            actual.Should().Be(expected);
            actual.IsSpecified.Should().BeTrue();
        }

        [Fact]
        public void ParseThrowsOnEmptyString()
        {
            // Arrange.
            var act = () => UserId.Parse(string.Empty);

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
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = UserId.Parse(null);
#pragma warning restore CS8625
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
            bool success = UserId.TryParse(raw, out UserId result);

            // Assert.
            success.Should().BeTrue();
            result.IsSpecified.Should().BeTrue();
            result.Value.Should().Be(Guid.Parse(raw));
        }

        [Fact]
        public void TryParseInvalidStringReturnsFalseAndDefault()
        {
            // Arrange. / Act.
            bool success = UserId.TryParse("not-a-guid", out UserId result);

            // Assert.
            success.Should().BeFalse();
            result.Should().Be(default(UserId));
            result.IsSpecified.Should().BeFalse();
        }

        [Fact]
        public void TryParseNullReturnsFalseAndDefault()
        {
            // Arrange. / Act.
            bool success = UserId.TryParse(null, out UserId result);

            // Assert.
            success.Should().BeFalse();
            result.Should().Be(default(UserId));
        }

        [Fact]
        public void GeneratorCreateUserIdRoundTripsExplicitRaw()
        {
            // Arrange.
            string raw = _generator.GenerateRawId();

            // Act.
            UserId userId = _generator.CreateUserId(raw);

            // Assert.
            userId.IsSpecified.Should().BeTrue();
            userId.Value.Should().Be(Guid.Parse(raw));
        }
    }
}
