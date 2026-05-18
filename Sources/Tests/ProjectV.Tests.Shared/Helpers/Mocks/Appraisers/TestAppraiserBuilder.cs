using Acolyte.Assertions;
using ProjectV.Appraisers;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Appraisers
{
    /// <summary>
    /// Builder for <see cref="IAppraiser" /> test doubles backed by
    /// <see cref="NSubstitute" /> (Decision D-33). One file per interface;
    /// downstream test plans add sibling builders following the same shape.
    /// </summary>
    public sealed class TestAppraiserBuilder
    {
        private Func<BasicInfo, RatingDataContainer>? _getRatingsHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestAppraiserBuilder" />
        /// class. No behavior is configured until one of the <c>With*</c>
        /// methods is called.
        /// </summary>
        public TestAppraiserBuilder()
        {
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="IAppraiser" /> substitute with no configured behavior.
        /// </summary>
        public static IAppraiser CreateWithoutSetup()
        {
            return new TestAppraiserBuilder().Build();
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
        /// whatever <see cref="NSubstitute" /> would by default
        /// (<c>null</c> for reference types).
        /// </summary>
        public IAppraiser Build()
        {
            var substitute = Substitute.For<IAppraiser>();

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
