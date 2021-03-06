using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using ProjectV.Models.Internal;
using ProjectV.Models.Data;

namespace ProjectV.Appraisers.Tests
{
    public sealed class AppraiserTests
    {
        public AppraiserTests()
        {
        }

        [Fact]
        public void CheckTagPropertyDefaultValue()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            string actualValue = appraiser.Tag;

            string expectedValue = $"Appraiser<{nameof(BasicInfo)}>";

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckTypeIdPropertyDefaultValue()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Type actualValue = appraiser.TypeId;

            Type expectedValue = typeof(BasicInfo);

            Assert.NotNull(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckRatingNamePropertyDefaultValue()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            string actualValue = appraiser.RatingName;

            const string expectedValue = "Common rating";

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckRatingIdPropertyDefaultValue()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid actualValue = appraiser.RatingId;

            Guid expectedValue = Guid.Empty;
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void SetRatingIdPropertyAndCompare()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid expectedValue = Guid.NewGuid();
            appraiser.RatingId = expectedValue;

            Guid actualValue = appraiser.RatingId;
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRatingsThrowsExceptionBecauseOfRatingIdIsUnspecified()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo();

            Assert.Throws<InvalidOperationException>(
                () => appraiser.GetRatings(rawDataContainer, outputResults: false)
            );
            Assert.Throws<InvalidOperationException>(
                () => appraiser.GetRatings(rawDataContainer, outputResults: true)
            );
        }

        [Fact]
        public void GetRatingsThrowsExceptionBecauseOfNullContainer()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => appraiser.GetRatings(rawDataContainer: null, outputResults: false)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            );
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => appraiser.GetRatings(rawDataContainer: null, outputResults: true)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            );
        }

        [Fact]
        public void CallGetRatingsWithEmptyConteiner()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid ratingId = Guid.NewGuid();
            appraiser.RatingId = ratingId;

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo();

            IReadOnlyList<ResultInfo> actualValue = appraiser.GetRatings(
                rawDataContainer, outputResults: false
            );

            IReadOnlyList<ResultInfo> expectedValue = new List<ResultInfo>();

            Assert.NotNull(actualValue);
            Assert.Empty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithOneItem()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid ratingId = Guid.NewGuid();
            appraiser.RatingId = ratingId;

            var item = new BasicInfo(
                thingId: 1, title: "Title", voteCount: 10, voteAverage: 9.9
            );

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo(item);

            IReadOnlyList<ResultInfo> actualValue = appraiser.GetRatings(
                rawDataContainer, outputResults: false
            );

            IReadOnlyList<ResultInfo> expectedValue =
                TestDataCreator.CreateExpectedValueForBasicInfo(
                    ratingId, rawDataContainer, item
                );

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Single(actualValue);
            Assert.Equal(expectedValue.Single(), actualValue.Single());
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithThreeItems()
        {
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid ratingId = Guid.NewGuid();
            appraiser.RatingId = ratingId;

            var item1 = new BasicInfo(
                thingId: 1, title: "Title-1", voteCount: 11, voteAverage: 9.7
            );
            var item2 = new BasicInfo(
               thingId: 2, title: "Title-2", voteCount: 12, voteAverage: 9.8
           );
            var item3 = new BasicInfo(
               thingId: 3, title: "Title-3", voteCount: 13, voteAverage: 9.9
           );

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo(item1, item2, item3);

            IReadOnlyList<ResultInfo> actualValue = appraiser.GetRatings(
                rawDataContainer, outputResults: false
            );

            IReadOnlyList<ResultInfo> expectedValue =
                TestDataCreator.CreateExpectedValueForBasicInfo(
                    ratingId, rawDataContainer, item1, item2, item3
                );

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.All(expectedValue, resultInfo => actualValue.Contains(resultInfo));
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
            IAppraiser appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid ratingId = Guid.NewGuid();
            appraiser.RatingId = ratingId;

            IReadOnlyList<BasicInfo> items =
                TestDataCreator.CreateBasicInfoListRandomly(itemsCount);

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo(items);

            IReadOnlyList<ResultInfo> actualValue = appraiser.GetRatings(
                rawDataContainer, outputResults: false
            );

            IReadOnlyList<ResultInfo> expectedValue =
                TestDataCreator.CreateExpectedValueForBasicInfo(
                    ratingId, rawDataContainer, items
                );

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.All(expectedValue, resultInfo => actualValue.Contains(resultInfo));
        }
    }
}
