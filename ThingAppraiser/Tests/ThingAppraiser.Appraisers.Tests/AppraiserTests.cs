using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.Tests
{
    public sealed class AppraiserTests
    {
        public AppraiserTests()
        {
        }

        [Fact]
        public void CheckTagPropertyDefaultValue()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            string actualValue = mock.Object.Tag;
            mock.Verify(x => x.Tag, Times.Once);

            const string expectedValue = nameof(Appraiser);

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckTypeIdPropertyDefaultValue()
        {
            var mock = new Mock<AppraiserBase>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Type actualValue = mock.Object.TypeId;
            mock.Verify(x => x.TypeId, Times.Once);

            Type expectedValue = typeof(BasicInfo);

            Assert.NotNull(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckRatingNamePropertyDefaultValue()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            string actualValue = mock.Object.RatingName;
            mock.Verify(x => x.RatingName, Times.Once);

            const string expectedValue = "Common rating";

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckRatingIdPropertyDefaultValue()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid actualValue = mock.Object.RatingId;

            Guid expectedValue = Guid.Empty;
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void SetRatingIdPropertyAndCompare()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid expectedValue = Guid.NewGuid();
            mock.Object.RatingId = expectedValue;

            Guid actualValue = mock.Object.RatingId;
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetRatingsThrowsExceptionBecauseOfRatingIdIsUnspecified()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo();

            Assert.Throws<InvalidOperationException>(
                () => mock.Object.GetRatings(rawDataContainer, outputResults: false)
            );
            Assert.Throws<InvalidOperationException>(
                () => mock.Object.GetRatings(rawDataContainer, outputResults: true)
            );
        }

        [Fact]
        public void GetRatingsThrowsExceptionBecauseOfNullContainer()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => mock.Object.GetRatings(rawDataContainer: null, outputResults: false)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            );
            Assert.Throws<ArgumentNullException>(
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                () => mock.Object.GetRatings(rawDataContainer: null, outputResults: true)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            );
        }

        [Fact]
        public void CallGetRatingsWithEmptyConteiner()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid ratingId = Guid.NewGuid();
            mock.Object.RatingId = ratingId;

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo();

            IReadOnlyList<ResultInfo> actualValue = mock.Object.GetRatings(
                rawDataContainer, outputResults: false
            );
            mock.Verify(x => x.GetRatings(It.IsNotNull<RawDataContainer>(), It.IsAny<bool>()),
                        Times.Once);

            IReadOnlyList<ResultInfo> expectedValue = new List<ResultInfo>();

            Assert.NotNull(actualValue);
            Assert.Empty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CallGetRatingsWithConteinerWithOneItem()
        {
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid ratingId = Guid.NewGuid();
            mock.Object.RatingId = ratingId;

            var item = new BasicInfo(
                thingId: 1, title: "Title", voteCount: 10, voteAverage: 9.9
            );

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo(item);

            IReadOnlyList<ResultInfo> actualValue = mock.Object.GetRatings(
                rawDataContainer, outputResults: false
            );
            mock.Verify(x => x.GetRatings(It.IsNotNull<RawDataContainer>(), It.IsAny<bool>()),
                        Times.Once);

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
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid ratingId = Guid.NewGuid();
            mock.Object.RatingId = ratingId;

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

            IReadOnlyList<ResultInfo> actualValue = mock.Object.GetRatings(
                rawDataContainer, outputResults: false
            );
            mock.Verify(x => x.GetRatings(It.IsNotNull<RawDataContainer>(), It.IsAny<bool>()),
                        Times.Once);

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
            var mock = new Mock<Appraiser>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid ratingId = Guid.NewGuid();
            mock.Object.RatingId = ratingId;

            IReadOnlyList<BasicInfo> items =
                TestDataCreator.CreateBasicInfoListRandomly(itemsCount);

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo(items);

            IReadOnlyList<ResultInfo> actualValue = mock.Object.GetRatings(
                rawDataContainer, outputResults: false
            );
            mock.Verify(x => x.GetRatings(It.IsNotNull<RawDataContainer>(), It.IsAny<bool>()),
                        Times.Once);

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
