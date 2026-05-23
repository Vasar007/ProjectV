using System;
using AwesomeAssertions;
using ProjectV.Appraisers.Appraisals.Movie.Tmdb;
using ProjectV.Models.Data;
using Xunit;

namespace ProjectV.Appraisers.Tests.AppraisersExtensions
{
    /// <summary>
    /// Rating-computation accuracy for the canonical movie-common appraiser
    /// path: <see cref="Appraiser{T}" /> of <see cref="TmdbMovieInfo" />
    /// composed with <see cref="TmdbCommonAppraisal" />, which returns the
    /// movie's TMDb popularity as its rating value.
    /// </summary>
    /// <remarks>
    /// The plan named this file <c>MovieCommonAppraiserTests</c>; ProjectV
    /// does NOT declare a <c>MovieCommonAppraiser</c> type — the production
    /// shape is <c>Appraiser&lt;TmdbMovieInfo&gt;(new TmdbCommonAppraisal())</c>.
    /// The unit boundary is the appraiser class composed with its strategy;
    /// the strategy is exercised directly (not mocked) because the strategy
    /// is the source of the rating value.
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class MovieCommonAppraiserTests
    {
        public MovieCommonAppraiserTests()
        {
        }

        private static Appraiser<TmdbMovieInfo> CreateSut()
        {
            return new Appraiser<TmdbMovieInfo>(new TmdbCommonAppraisal());
        }

        private static TmdbMovieInfo CreateMovie(double popularity = 7.5,
            double voteAverage = 8.1, int voteCount = 1234)
        {
            return new TmdbMovieInfo(
                thingId: 42,
                title: "Inception",
                voteCount: voteCount,
                voteAverage: voteAverage,
                overview: "A heist inside dreams.",
                releaseDate: new DateTime(2010, 7, 16),
                popularity: popularity,
                adult: false,
                genreIds: new[] { 28, 878 },
                posterPath: "/inception.jpg"
            );
        }

        [Fact]
        public void TagPropertyMatchesGenericTypeName()
        {
            // Arrange.
            var sut = CreateSut();
            var expected = $"Appraiser<{nameof(TmdbMovieInfo)}>";

            // Act.
            var actual = sut.Tag;

            // Assert.
            actual.Should().NotBeNull();
            actual.Should().NotBeEmpty();
            actual.Should().Be(expected);
        }

        [Fact]
        public void TypeIdMatchesGenericArgument()
        {
            // Arrange.
            var sut = CreateSut();

            // Act.
            var actual = sut.TypeId;

            // Assert.
            actual.Should().Be(typeof(TmdbMovieInfo));
        }

        [Fact]
        public void RatingNameMatchesAppraisalRatingName()
        {
            // Arrange.
            var sut = CreateSut();
            const string expected = "Rating based on popularity";

            // Act.
            var actual = sut.RatingName;

            // Assert.
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetRatingsReturnsPopularityFromAppraisal()
        {
            // Arrange.
            var sut = CreateSut();
            var movie = CreateMovie(popularity: 12.34);

            // Act.
            var actual = sut.GetRatings(movie, outputResults: false);

            // Assert.
            actual.Should().NotBeNull();
            actual.RatingValue.Should().Be(12.34);
            actual.DataHandler.Should().BeSameAs(movie);
            actual.RatingId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetRatingsAssignsEmptyRatingIdToAllResults()
        {
            // Arrange.
            var sut = CreateSut();
            var first = CreateMovie(popularity: 1.0);
            var second = CreateMovie(popularity: 9.9);

            // Act.
            var firstRating = sut.GetRatings(first, outputResults: false);
            var secondRating = sut.GetRatings(second, outputResults: false);

            // Assert.
            firstRating.RatingId.Should().Be(Guid.Empty);
            secondRating.RatingId.Should().Be(Guid.Empty);
            firstRating.RatingValue.Should().NotBe(secondRating.RatingValue);
        }

        [Fact]
        public void GetRatingsThrowsForNullEntity()
        {
            // Arrange.
            var sut = CreateSut();

            // Act. / Assert.
            var act = () =>
            {
                sut.GetRatings(entityInfo: null!, outputResults: false);
            };
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("entityInfo");
        }

        [Fact]
        public void GetRatingsThrowsForBaseBasicInfoBecauseAppraiserExpectsTmdb()
        {
            // Arrange.
            var sut = CreateSut();
            var basicInfo = new BasicInfo(
                thingId: 1, title: "Generic", voteCount: 10, voteAverage: 7.0);

            // Act.
            var act = () => sut.GetRatings(basicInfo, outputResults: false);

            // Assert.
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ConstructorThrowsForNullAppraisal()
        {
            // Arrange. / Act. / Assert.
            var act = () =>
            {
                _ = new Appraiser<TmdbMovieInfo>(appraisal: null!);
            };
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("appraisal");
        }
    }
}
