using System;
using System.Collections.Generic;
using AwesomeAssertions;
using ProjectV.Appraisers.Appraisals;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using Xunit;

namespace ProjectV.Appraisers.Tests.AppraisersExtensions
{
    /// <summary>
    /// Rating-computation accuracy for the canonical movie-normalized
    /// appraiser path: <see cref="Appraiser{T}" /> of <see cref="BasicInfo" />
    /// composed with <see cref="BasicAppraisalNormalized" />, which projects
    /// vote-count and vote-average onto a min-max normalised scale.
    /// </summary>
    /// <remarks>
    /// ProjectV does NOT declare a <c>MovieNormalizedAppraiser</c> type —
    /// the production shape is
    /// <c>Appraiser&lt;BasicInfo&gt;(new BasicAppraisalNormalized())</c>,
    /// and this file covers that composition. The
    /// appraisal MUST be prepared via
    /// <see cref="BasicAppraisalNormalized.PrepareCalculation" /> with a
    /// <see cref="RawDataContainer" /> that carries
    /// <see cref="MinMaxDenominator" />s keyed by
    /// <c>nameof(BasicInfo.VoteCount)</c> and
    /// <c>nameof(BasicInfo.VoteAverage)</c>; otherwise it throws
    /// <see cref="InvalidOperationException" />.
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class MovieNormalizedAppraiserTests : BaseMockTest
    {
        private readonly BasicInfoGenerator _generator;

        public MovieNormalizedAppraiserTests()
        {
            _generator = BasicInfoGenerator.Instance;
        }

        private static BasicAppraisalNormalized CreateAppraisal()
        {
            return new BasicAppraisalNormalized();
        }

        private static Appraiser<BasicInfo> CreateSut(BasicAppraisalNormalized appraisal)
        {
            return new Appraiser<BasicInfo>(appraisal);
        }

        /// <summary>
        /// Creates a <see cref="BasicInfo" /> with explicit values via
        /// <see cref="BasicInfoGenerator" />. Per-class helper so test bodies
        /// do not create test data inline or call generators directly.
        /// </summary>
        private BasicInfo CreateBasicInfo(
            int thingId, string title, int voteCount, double voteAverage)
        {
            return _generator.CreateBasicInfo(
                thingId: thingId,
                title: title,
                voteCount: voteCount,
                voteAverage: voteAverage);
        }

        private IReadOnlyList<BasicInfo> CreateMovieBatch()
        {
            return new[]
            {
                CreateBasicInfo(thingId: 1, title: "A", voteCount: 10, voteAverage: 5.0),
                CreateBasicInfo(thingId: 2, title: "B", voteCount: 50, voteAverage: 7.0),
                CreateBasicInfo(thingId: 3, title: "C", voteCount: 90, voteAverage: 9.0),
            };
        }

        private static RawDataContainer CreateRawDataContainer(
            IReadOnlyList<BasicInfo> items)
        {
            var container = new RawDataContainer(items);
            container.AddParameter(
                nameof(BasicInfo.VoteCount),
                MinMaxDenominator.CreateForCollection(items, b => b.VoteCount));
            container.AddParameter(
                nameof(BasicInfo.VoteAverage),
                MinMaxDenominator.CreateForCollection(items, b => b.VoteAverage));
            return container;
        }

        [Fact]
        public void TagPropertyReflectsBasicInfoTypeParameter()
        {
            // Arrange.
            var sut = CreateSut(CreateAppraisal());
            var expected = $"Appraiser<{nameof(BasicInfo)}>";

            // Act.
            var actual = sut.Tag;

            // Assert.
            actual.Should().Be(expected);
        }

        [Fact]
        public void RatingNameMatchesAppraisalRatingName()
        {
            // Arrange.
            var sut = CreateSut(CreateAppraisal());

            // Act.
            var actual = sut.RatingName;

            // Assert.
            actual.Should().Be("Common rating");
        }

        [Fact]
        public void GetRatingsAfterPrepareReturnsNormalisedSumOfMinAndMaxItem()
        {
            // Arrange.
            var items = CreateMovieBatch();
            var appraisal = CreateAppraisal();
            appraisal.PrepareCalculation(CreateRawDataContainer(items));
            var sut = CreateSut(appraisal);

            // Act.
            var minRating = sut.GetRatings(items[0], outputResults: false);
            var maxRating = sut.GetRatings(items[2], outputResults: false);

            // Assert. min-item maps to 0 + 0 = 0; max-item maps to 1 + 1 = 2.
            minRating.RatingValue.Should().Be(0.0);
            maxRating.RatingValue.Should().Be(2.0);
        }

        [Fact]
        public void GetRatingsAfterPrepareReturnsBoundedValueForMiddleItem()
        {
            // Arrange.
            var items = CreateMovieBatch();
            var appraisal = CreateAppraisal();
            appraisal.PrepareCalculation(CreateRawDataContainer(items));
            var sut = CreateSut(appraisal);

            // Act.
            var middle = sut.GetRatings(items[1], outputResults: false);

            // Assert. Middle item lies inside the [0, 2] envelope produced
            // by the min-max normalisation.
            middle.RatingValue.Should().BeGreaterThanOrEqualTo(0.0);
            middle.RatingValue.Should().BeLessThanOrEqualTo(2.0);
        }

        [Fact]
        public void GetRatingsWithoutPrepareThrowsInvalidOperation()
        {
            // Arrange.
            var sut = CreateSut(CreateAppraisal());
            BasicInfo entity = CreateBasicInfo(
                thingId: 1, title: "X", voteCount: 10, voteAverage: 5.0);

            // Act.
            var act = () => sut.GetRatings(entity, outputResults: false);

            // Assert. BasicAppraisalNormalized requires PrepareCalculation
            // to populate the MinMaxDenominator fields.
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetRatingsThrowsForNullEntity()
        {
            // Arrange.
            var appraisal = CreateAppraisal();
            appraisal.PrepareCalculation(CreateRawDataContainer(CreateMovieBatch()));
            var sut = CreateSut(appraisal);

            // Act.
            var act = () =>
            {
                sut.GetRatings(entityInfo: null!, outputResults: false);
            };

            // Assert.
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("entityInfo");
        }

        [Fact]
        public void PrepareCalculationThrowsForNullContainer()
        {
            // Arrange.
            var appraisal = CreateAppraisal();

            // Act.
            var act = () =>
            {
                appraisal.PrepareCalculation(rawDataContainer: null!);
            };

            // Assert.
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("rawDataContainer");
        }
    }
}
