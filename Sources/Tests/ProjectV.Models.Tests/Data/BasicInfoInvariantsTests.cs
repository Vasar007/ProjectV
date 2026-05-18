using AwesomeAssertions;
using Newtonsoft.Json;
using ProjectV.Models.Data;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.Models.Tests.Data
{
    /// <summary>
    /// Unit tests for <see cref="BasicInfo" /> invariants — primitive
    /// property defaults, <see cref="BasicInfo.Kind" /> virtual default,
    /// equality semantics with floating-point tolerance, and the
    /// Newtonsoft.Json round-trip honoured by the
    /// <c>[JsonConstructor]</c>-annotated primary constructor.
    /// </summary>
    /// <remarks>
    /// The production <see cref="BasicInfo" /> primary ctor does NOT carry
    /// Acolyte <c>ThrowIfNull</c> guards (every other ctor in
    /// <c>ProjectV.Models</c> does — see CONVENTIONS.md). Testing the
    /// observed behaviour here, not the convention; if a future plan adds
    /// guards, the corresponding tests in this file should flip from
    /// "accepts null/empty" to "rejects null/empty".
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class BasicInfoInvariantsTests
    {
        private readonly BasicInfoGenerator _generator;

        public BasicInfoInvariantsTests()
        {
            _generator = new BasicInfoGenerator();
        }

        [Fact]
        public void PrimitiveDefaultsAcceptedByConstructor()
        {
            // Arrange. / Act.
            var info = new BasicInfo(
                thingId: 0,
                title: string.Empty,
                voteCount: 0,
                voteAverage: 0.0);

            // Assert.
            info.ThingId.Should().Be(0);
            info.Title.Should().BeEmpty();
            info.VoteCount.Should().Be(0);
            info.VoteAverage.Should().Be(0.0);
        }

        [Fact]
        public void KindDefaultsToTypeName()
        {
            // Arrange. / Act.
            var info = _generator.GenerateBasicInfo();

            // Assert.
            info.Kind.Should().Be(nameof(BasicInfo));
        }

        [Fact]
        public void ConstructorAssignsAllPropertiesFromArguments()
        {
            // Arrange.
            var info = _generator.CreateBasicInfo(
                thingId: 42, title: "Title", voteCount: 100, voteAverage: 9.5);

            // Assert.
            info.ThingId.Should().Be(42);
            info.Title.Should().Be("Title");
            info.VoteCount.Should().Be(100);
            info.VoteAverage.Should().Be(9.5);
        }

        [Fact]
        public void EqualsReturnsTrueForMemberwiseIdenticalInstances()
        {
            // Arrange.
            var left = _generator.CreateBasicInfo(
                thingId: 1, title: "Same", voteCount: 5, voteAverage: 7.7);
            var right = _generator.CreateBasicInfo(
                thingId: 1, title: "Same", voteCount: 5, voteAverage: 7.7);

            // Act. / Assert.
            left.Should().Be(right);
            (left == right).Should().BeTrue();
            (left != right).Should().BeFalse();
            left.GetHashCode().Should().Be(right.GetHashCode());
        }

        [Fact]
        public void EqualsAppliesToleranceOnVoteAverage()
        {
            // Arrange. BasicInfo.IsEqual uses Math.Abs(diff) < 1e-6.
            var left = _generator.CreateBasicInfo(
                thingId: 1, title: "Same", voteCount: 5, voteAverage: 7.7);
            var right = _generator.CreateBasicInfo(
                thingId: 1, title: "Same", voteCount: 5,
                voteAverage: 7.7 + 1e-9);

            // Act. / Assert.
            left.Should().Be(right);
        }

        [Fact]
        public void EqualsReturnsFalseWhenAnyFieldDiffers()
        {
            // Arrange.
            var baseline = _generator.CreateBasicInfo(
                thingId: 1, title: "T", voteCount: 5, voteAverage: 7.7);

            // Act.
            var differentId = _generator.CreateBasicInfo(
                thingId: 2, title: "T", voteCount: 5, voteAverage: 7.7);
            var differentTitle = _generator.CreateBasicInfo(
                thingId: 1, title: "U", voteCount: 5, voteAverage: 7.7);
            var differentVoteCount = _generator.CreateBasicInfo(
                thingId: 1, title: "T", voteCount: 6, voteAverage: 7.7);
            var differentVoteAverage = _generator.CreateBasicInfo(
                thingId: 1, title: "T", voteCount: 5, voteAverage: 7.8);

            // Assert.
            baseline.Should().NotBe(differentId);
            baseline.Should().NotBe(differentTitle);
            baseline.Should().NotBe(differentVoteCount);
            baseline.Should().NotBe(differentVoteAverage);
        }

        [Fact]
        public void EqualsHandlesNullAndSelfReference()
        {
            // Arrange.
            var info = _generator.GenerateBasicInfo();

            // Act. / Assert.
            info.Equals(info).Should().BeTrue();
            info.Equals(other: null).Should().BeFalse();
        }

        [Fact]
        public void NewtonsoftJsonRoundTripsCompact()
        {
            // Arrange.
            var expected = _generator.CreateBasicInfo(
                thingId: 7, title: "Round-Trip", voteCount: 42, voteAverage: 8.4);

            // Act.
            string json = JsonConvert.SerializeObject(expected);
            BasicInfo? actual = JsonConvert.DeserializeObject<BasicInfo>(json);

            // Assert.
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }

        [Fact]
        public void NewtonsoftJsonRoundTripsPrettyPrinted()
        {
            // Arrange.
            var expected = _generator.CreateBasicInfo(
                thingId: 7, title: "Round-Trip", voteCount: 42, voteAverage: 8.4);

            // Act.
            string json = JsonConvert.SerializeObject(expected, Formatting.Indented);
            BasicInfo? actual = JsonConvert.DeserializeObject<BasicInfo>(json);

            // Assert.
            actual.Should().NotBeNull();
            actual.Should().Be(expected);
        }

        [Fact]
        public void NewtonsoftJsonPreservesKindDiscriminator()
        {
            // Arrange.
            var info = _generator.GenerateBasicInfo();

            // Act.
            string json = JsonConvert.SerializeObject(info);

            // Assert.
            json.Should().Contain($"\"Kind\":\"{nameof(BasicInfo)}\"");
        }
    }
}
