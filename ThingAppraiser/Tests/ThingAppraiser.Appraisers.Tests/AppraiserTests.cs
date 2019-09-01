using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Xunit;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.Data;
using Moq.Protected;
using ThingAppraiser.Appraisers.Appraisals;

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

            var basicInfo = new BasicInfo(
                thingId: 1, title: "Title", voteCount: 10, voteAverage: 9.9
            );

            RawDataContainer rawDataContainer =
                TestDataCreator.CreateRawDataContainerWithBasicInfo(basicInfo);

            IReadOnlyList<ResultInfo> actualValue = mock.Object.GetRatings(
                rawDataContainer, outputResults: false
            );
            mock.Verify(x => x.GetRatings(It.IsNotNull<RawDataContainer>(), It.IsAny<bool>()),
                        Times.Once);

            var appraisal = new BasicAppraisal(rawDataContainer);

            double expectedRating = appraisal.CalculateRating(basicInfo);
            var expectedItem = new ResultInfo(basicInfo.ThingId, expectedRating, ratingId);

            IReadOnlyList<ResultInfo> expectedValue = new List<ResultInfo> { expectedItem };
            Assert.Equal(expectedValue, actualValue);
        }
    }
}
