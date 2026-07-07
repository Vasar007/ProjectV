namespace ProjectV.Tests.Shared.ForTests
{
    /// <summary>
    /// Base class for tests of GUID-backed identifier value-objects
    /// (e.g. <c>JobId</c>, <c>UserId</c>) that follow the ProjectV wrapper
    /// convention: static <c>None</c> / <c>Create</c> / <c>Wrap</c> /
    /// <c>Parse</c> / <c>TryParse</c> members plus <c>Value</c> and
    /// <c>IsSpecified</c> instance surface. The wrapper types share no
    /// common interface (each is an independent
    /// <c>readonly record struct</c>), so concrete test classes bridge the
    /// per-type static surface and the matching test-data generator through
    /// the abstract hooks below.
    /// </summary>
    /// <typeparam name="TId">Wrapper type under test.</typeparam>
    public abstract class BaseGuidWrapperTests<TId> : BaseTest
        where TId : struct, IEquatable<TId>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="BaseGuidWrapperTests{TId}" /> class.
        /// </summary>
        protected BaseGuidWrapperTests()
        {
        }

        #region Wrapper Surface Hooks

        /// <summary>
        /// Returns the wrapper's <c>None</c> sentinel.
        /// </summary>
        protected abstract TId None { get; }

        /// <summary>
        /// Creates a fresh wrapper via the static <c>Create</c> factory.
        /// </summary>
        protected abstract TId Create();

        /// <summary>
        /// Wraps a raw <see cref="Guid" /> via the static <c>Wrap</c>
        /// factory.
        /// </summary>
        /// <param name="id">Raw GUID to wrap.</param>
        protected abstract TId Wrap(Guid id);

        /// <summary>
        /// Parses a raw GUID string via the static <c>Parse</c> factory.
        /// </summary>
        /// <param name="rawId">Raw GUID string.</param>
        protected abstract TId Parse(string rawId);

        /// <summary>
        /// Tries to parse a raw GUID string via the static <c>TryParse</c>
        /// method.
        /// </summary>
        /// <param name="rawId">Raw GUID string (possibly invalid).</param>
        /// <param name="result">Parsed wrapper or <c>default</c>.</param>
        protected abstract bool TryParse(string? rawId, out TId result);

        /// <summary>
        /// Reads the wrapper's <c>Value</c> property.
        /// </summary>
        /// <param name="id">Wrapper instance.</param>
        protected abstract Guid GetValue(TId id);

        /// <summary>
        /// Reads the wrapper's <c>IsSpecified</c> property.
        /// </summary>
        /// <param name="id">Wrapper instance.</param>
        protected abstract bool GetIsSpecified(TId id);

        #endregion

        #region Generator Hooks

        /// <summary>
        /// Generates a wrapper with a random value via the matching
        /// test-data generator.
        /// </summary>
        protected abstract TId GenerateId();

        /// <summary>
        /// Creates a wrapper from an explicit raw GUID string via the
        /// matching test-data generator.
        /// </summary>
        /// <param name="rawId">Raw GUID string.</param>
        protected abstract TId CreateId(string rawId);

        /// <summary>
        /// Generates a raw GUID string via the matching test-data generator.
        /// </summary>
        protected abstract string GenerateRawId();

        #endregion

        [Fact]
        public void NoneIsEqualToDefault()
        {
            // Arrange.
            TId @default = default;

            // Act.
            TId none = None;

            // Assert.
            none.Should().Be(@default);
            GetValue(none).Should().Be(Guid.Empty);
        }

        [Fact]
        public void NoneIsSpecifiedReturnsFalse()
        {
            // Arrange. / Act.
            bool isSpecified = GetIsSpecified(None);

            // Assert.
            isSpecified.Should().BeFalse();
        }

        [Fact]
        public void CreateReturnsSpecifiedNonEmptyId()
        {
            // Arrange. / Act.
            TId id = Create();

            // Assert.
            GetIsSpecified(id).Should().BeTrue();
            GetValue(id).Should().NotBe(Guid.Empty);
            id.Should().NotBe(None);
        }

        [Fact]
        public void CreateReturnsDistinctIdsOnEachCall()
        {
            // Arrange. / Act.
            TId first = Create();
            TId second = Create();

            // Assert.
            first.Should().NotBe(second);
        }

        [Fact]
        public void WrapWithNonEmptyGuidReturnsSpecifiedId()
        {
            // Arrange.
            var raw = Guid.NewGuid();

            // Act.
            TId id = Wrap(raw);

            // Assert.
            GetValue(id).Should().Be(raw);
            GetIsSpecified(id).Should().BeTrue();
        }

        [Fact]
        public void WrapWithEmptyGuidThrowsArgumentException()
        {
            // Arrange.
            var act = () => Wrap(Guid.Empty);

            // Act. / Assert.
            act.Should().Throw<ArgumentException>()
               .WithParameterName("id");
        }

        [Fact]
        public void ParseRoundTripsThroughGenerator()
        {
            // Arrange.
            TId expected = GenerateId();
            string raw = GetValue(expected).ToString();

            // Act.
            TId actual = Parse(raw);

            // Assert.
            actual.Should().Be(expected);
            GetIsSpecified(actual).Should().BeTrue();
        }

        [Fact]
        public void ParseThrowsOnEmptyString()
        {
            // Arrange.
            var act = () => Parse(string.Empty);

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
                _ = Parse(null!);
            };

            // Act. / Assert.
            act.Should().Throw<ArgumentException>()
               .WithParameterName("rawId");
        }

        [Fact]
        public void TryParseValidGuidReturnsTrueAndPopulatesResult()
        {
            // Arrange.
            string raw = GenerateRawId();

            // Act.
            bool success = TryParse(raw, out TId result);

            // Assert.
            success.Should().BeTrue();
            GetIsSpecified(result).Should().BeTrue();
            GetValue(result).Should().Be(Guid.Parse(raw));
        }

        [Fact]
        public void TryParseInvalidStringReturnsFalseAndDefault()
        {
            // Arrange. / Act.
            bool success = TryParse("not-a-guid", out TId result);

            // Assert.
            success.Should().BeFalse();
            result.Should().Be(default(TId));
            GetIsSpecified(result).Should().BeFalse();
        }

        [Fact]
        public void TryParseNullReturnsFalseAndDefault()
        {
            // Arrange. / Act.
            bool success = TryParse(null, out TId result);

            // Assert.
            success.Should().BeFalse();
            result.Should().Be(default(TId));
        }

        [Fact]
        public void GeneratorCreateIdRoundTripsExplicitRaw()
        {
            // Arrange.
            string raw = GenerateRawId();

            // Act.
            TId id = CreateId(raw);

            // Assert.
            GetIsSpecified(id).Should().BeTrue();
            GetValue(id).Should().Be(Guid.Parse(raw));
        }
    }
}
