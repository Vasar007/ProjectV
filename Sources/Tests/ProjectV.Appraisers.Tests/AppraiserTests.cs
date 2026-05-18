using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using Xunit;

namespace ProjectV.Appraisers.Tests
{
    [Trait("Category", "Unit")]
    public sealed class AppraiserTests
    {
        public AppraiserTests()
        {
        }

        [Fact]
        public void CheckTagPropertyDefaultValue()
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();
            string expectedValue = $"Appraiser<{nameof(BasicInfo)}>";

            // Act.
            string actualValue = appraiser.Tag;

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Should().NotBeEmpty();
            actualValue.Should().Be(expectedValue);
        }

        [Fact]
        public void CheckTypeIdPropertyDefaultValue()
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();
            Type expectedValue = typeof(BasicInfo);

            // Act.
            Type actualValue = appraiser.TypeId;

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Should().Be(expectedValue);
        }

        [Fact]
        public void CheckRatingNamePropertyDefaultValue()
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();
            const string expectedValue = "Common rating";

            // Act.
            string actualValue = appraiser.RatingName;

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Should().NotBeEmpty();
            actualValue.Should().Be(expectedValue);
        }

        [Fact]
        public void GetRatingsThrowsExceptionBecauseOfNullParameter()
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            // Act. / Assert.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var actWithoutOutput = () => appraiser.GetRatings(entityInfo: null, outputResults: false);
            actWithoutOutput.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("entityInfo");

            var actWithOutput = () => appraiser.GetRatings(entityInfo: null, outputResults: true);
            actWithOutput.Should()
                .Throw<ArgumentNullException>()
                .WithParameterName("entityInfo");
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithOneItem()
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();
            Guid ratingId = Guid.Empty;
            var item = new BasicInfo(
                thingId: 1, title: "Title", voteCount: 10, voteAverage: 9.9
            );
            var expectedValue = TestDataCreator
                .CreateExpectedValueForBasicInfo(ratingId, item)
                .Single();

            // Act.
            var actualValue = appraiser.GetRatings(item, outputResults: false);

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Should().Be(expectedValue);
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithThreeItems()
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();
            Guid ratingId = Guid.Empty;
            var item1 = new BasicInfo(
                thingId: 1, title: "Title-1", voteCount: 11, voteAverage: 9.7
            );
            var item2 = new BasicInfo(
                thingId: 2, title: "Title-2", voteCount: 12, voteAverage: 9.8
            );
            var item3 = new BasicInfo(
                thingId: 3, title: "Title-3", voteCount: 13, voteAverage: 9.9
            );
            var items = new[] { item1, item2, item3 };
            var expectedValue = TestDataCreator.CreateExpectedValueForBasicInfo(
                ratingId, item1, item2, item3
            );

            // Act.
            var actualValue = new List<RatingDataContainer>();
            for (int index = 0; index < items.Length; ++index)
            {
                var actualRating = appraiser.GetRatings(items[index], outputResults: false);
                actualValue.Add(actualRating);
            }

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Should().NotBeEmpty();
            actualValue.Should().BeEquivalentTo(expectedValue);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(100)]
        public void CallGetRatingsWithConteinerWithRandomData(int itemsCount)
        {
            // Arrange.
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();
            Guid ratingId = Guid.Empty;
            var items = TestDataCreator.CreateBasicInfoListRandomly(itemsCount);
            var expectedValue = TestDataCreator.CreateExpectedValueForBasicInfo(ratingId, items);

            // Act.
            var actualValue = new List<RatingDataContainer>();
            for (int index = 0; index < items.Count; ++index)
            {
                var actualRating = appraiser.GetRatings(items[index], outputResults: false);
                actualValue.Add(actualRating);
            }

            // Assert.
            actualValue.Should().NotBeNull();
            actualValue.Should().NotBeEmpty();
            actualValue.Should().BeEquivalentTo(expectedValue);
        }
    }
}
