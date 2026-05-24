using Acolyte.Assertions;
using AutoFixture;
using ProjectV.Appraisers;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Appraisers
{
    /// <summary>
    /// Builder for <see cref="IAppraiser" /> test doubles backed by
    /// AutoFixture + NSubstitute. One file per interface;
    /// downstream test plans add sibling builders following the same shape.
    /// </summary>
    public sealed class TestAppraiserBuilder
    {
        private readonly IFixture _fixture;

        private Type? _typeId;
        private string? _tag;
        private Func<BasicInfo, RatingDataContainer>? _getRatingsHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAppraiserBuilder" />
        /// class. No behavior is configured until one of the <c>With*</c>
        /// methods is called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestAppraiserBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="IAppraiser" /> substitute with no configured behavior.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static IAppraiser CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestAppraiserBuilder(fixture).Build();
        }

        /// <summary>
        /// Overrides the <see cref="IAppraiser.TypeId" /> value returned by the
        /// substitute.
        /// </summary>
        /// <param name="typeId">Type id. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraiserBuilder WithTypeId(Type typeId)
        {
            typeId.ThrowIfNull(nameof(typeId));

            _typeId = typeId;
            return this;
        }

        /// <summary>
        /// Overrides the <see cref="IAppraiser.Tag" /> value returned by the
        /// substitute.
        /// </summary>
        /// <param name="tag">Tag value. Must not be <c>null</c>/whitespace.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraiserBuilder WithTag(string tag)
        {
            tag.ThrowIfNullOrWhiteSpace(nameof(tag));

            _tag = tag;
            return this;
        }

        /// <summary>
        /// Configures the appraiser to return the supplied
        /// <paramref name="rating" /> for every call to
        /// <see cref="IAppraiser.GetRatings(BasicInfo, bool)" />.
        /// </summary>
        /// <param name="rating">Rating container to return.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraiserBuilder WithRating(RatingDataContainer rating)
        {
            rating.ThrowIfNull(nameof(rating));

            _getRatingsHandler = _ => rating;
            return this;
        }

        /// <summary>
        /// Configures the appraiser to compute a rating from the supplied
        /// <paramref name="handler" /> delegate. Useful for tests that need
        /// per-<see cref="BasicInfo" /> rating logic.
        /// </summary>
        /// <param name="handler">Delegate that produces a rating container.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestAppraiserBuilder WithRatingFactory(
            Func<BasicInfo, RatingDataContainer> handler)
        {
            handler.ThrowIfNull(nameof(handler));

            _getRatingsHandler = handler;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="IAppraiser" /> substitute. If no
        /// <c>With*</c> method has been called, the substitute returns
        /// whatever AutoFixture / NSubstitute would by default.
        /// </summary>
        public IAppraiser Build()
        {
            var substitute = _fixture.Create<IAppraiser>();

            if (_typeId is not null)
            {
                substitute.TypeId.Returns(_typeId);
            }

            if (_tag is not null)
            {
                substitute.Tag.Returns(_tag);
            }

            if (_getRatingsHandler is not null)
            {
                var handler = _getRatingsHandler;
                substitute
                    .GetRatings(Arg.Any<BasicInfo>(), Arg.Any<bool>())
                    .Returns(ci => handler(ci.ArgAt<BasicInfo>(0)));
            }

            return substitute;
        }
    }
}
