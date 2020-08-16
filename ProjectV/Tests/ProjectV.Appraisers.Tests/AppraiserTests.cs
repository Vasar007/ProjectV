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
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            string actualValue = appraiser.Tag;

            string expectedValue = $"AppraiserAsync<{nameof(BasicInfo)}>";

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckTypeIdPropertyDefaultValue()
        {
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Type actualValue = appraiser.TypeId;

            Type expectedValue = typeof(BasicInfo);

            Assert.NotNull(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckRatingNamePropertyDefaultValue()
        {
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            string actualValue = appraiser.RatingName;

            const string expectedValue = "Common rating";

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRatingsThrowsExceptionBecauseOfNullParameter()
        {
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Assert.Throws<ArgumentNullException>(
                "entityInfo",
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => appraiser.GetRatings(entityInfo: null, outputResults: false)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            );
            Assert.Throws<ArgumentNullException>(
                "entityInfo",
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => appraiser.GetRatings(entityInfo: null, outputResults: true)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            );
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithOneItem()
        {
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid ratingId = Guid.Empty;

            var item = new BasicInfo(
                thingId: 1, title: "Title", voteCount: 10, voteAverage: 9.9
            );

            var actualValue = appraiser.GetRatings(item, outputResults: false);

            var expectedValue = TestDataCreator.CreateExpectedValueForBasicInfo(ratingId, item)
                .Single();

            Assert.NotNull(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithThreeItems()
        {
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

            var actualValue = new List<RatingDataContainer>();
            for (int index = 0; index < items.Length; ++index)
            {
                var actualRating = appraiser.GetRatings(items[index], outputResults: false);
                actualValue.Add(actualRating);
            }

            var expectedValue = TestDataCreator.CreateExpectedValueForBasicInfo(
                ratingId, item1, item2, item3
            );

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
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
            var appraiser = TestAppraisersCreator.CreateBasicAppraiser();

            Guid ratingId = Guid.Empty;

            var items = TestDataCreator.CreateBasicInfoListRandomly(itemsCount);

            var actualValue = new List<RatingDataContainer>();
            for (int index = 0; index < items.Count; ++index)
            {
                var actualRating = appraiser.GetRatings(items[index], outputResults: false);
                actualValue.Add(actualRating);
            }

            var expectedValue = TestDataCreator.CreateExpectedValueForBasicInfo(ratingId, items);

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }
    }
}
