using System;
using Moq;
using Xunit;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Appraisers.Tests
{
    public sealed class AppraiserBaseTests
    {
        public AppraiserBaseTests()
        {
        }

        [Fact]
        public void CheckTagPropertyDefaultValue()
        {
            var mock = new Mock<AppraiserBase>(MockBehavior.Loose)
            {
                CallBase = true
            };

            string actualValue = mock.Object.Tag;
            mock.Verify(x => x.Tag, Times.Once);

            const string expectedValue = nameof(AppraiserBase);

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
            var mock = new Mock<AppraiserBase>(MockBehavior.Loose)
            {
                CallBase = true
            };

            string actualValue = mock.Object.RatingName;
            mock.Verify(x => x.RatingName, Times.Once);

            string expectedValue = $"{nameof(AppraiserBase)} has no rating calculation";

            Assert.NotNull(actualValue);
            Assert.NotEmpty(actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void CheckRatingIdPropertyDefaultValue()
        {
            var mock = new Mock<AppraiserBase>(MockBehavior.Loose)
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
            var mock = new Mock<AppraiserBase>(MockBehavior.Loose)
            {
                CallBase = true
            };

            Guid expectedValue = Guid.NewGuid();
            mock.Object.RatingId = expectedValue;

            Guid actualValue = mock.Object.RatingId;

            Assert.Equal(expectedValue, actualValue);
        }
    }
}
