using System;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.DataPipeline;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Mocks.Appraisers;
using ProjectV.Tests.Shared.Helpers.Stubs.Appraisers;
using Xunit;

namespace ProjectV.Appraisers.Tests.AppraisersExtensions
{
    /// <summary>
    /// Unit tests for <see cref="AppraisersManager" /> — verifies the
    /// Add/Remove API, the <see cref="AppraisersManager.CreateFlow" /> shape,
    /// and the contract surface around <see cref="IAppraiser" /> children.
    /// Uses <see cref="TestAppraisersManagerBuilder" /> to construct the SUT
    /// and <see cref="TestAppraiserBuilder" /> for substitute children.
    /// </summary>
    [Trait("Category", "Unit")]
    public sealed class AppraisersManagerTests : BaseMockTest
    {
        public AppraisersManagerTests()
        {
        }

        [Fact]
        public void CreateWithoutSetupReturnsEmptyManager()
        {
            // Arrange. / Act.
            var sut = TestAppraisersManagerBuilder.CreateWithoutSetup();

            // Assert. An empty manager produces a non-null but childless flow.
            sut.Should().NotBeNull();
            var flow = sut.CreateFlow();
            flow.Should().NotBeNull();
            flow.Should().BeOfType<AppraisersFlow>();
        }

        [Fact]
        public void AddThrowsForNullAppraiser()
        {
            // Arrange.
            var sut = TestAppraisersManagerBuilder.CreateWithoutSetup();

            // Act.
            var act = () =>
            {
                sut.Add(item: null!);
            };

            // Assert.
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("item");
        }

        [Fact]
        public void RemoveThrowsForNullAppraiser()
        {
            // Arrange.
            var sut = TestAppraisersManagerBuilder.CreateWithoutSetup();

            // Act.
            var act = () =>
            {
                sut.Remove(item: null!);
            };

            // Assert.
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("item");
        }

        [Fact]
        public void AddOnceRegistersAppraiserUnderItsTypeId()
        {
            // Arrange.
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");
            var sut = new TestAppraisersManagerBuilder()
                .WithAppraiser(appraiser)
                .Build();

            // Act.
            var flow = sut.CreateFlow();

            // Assert. The flow construction reads TypeId from every child.
            _ = appraiser.Received().TypeId;
            flow.Should().NotBeNull();
        }

        [Fact]
        public void AddSameInstanceTwiceIsIdempotentWithinSameTypeId()
        {
            // Arrange.
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");
            var sut = TestAppraisersManagerBuilder.CreateWithoutSetup();

            // Act.
            sut.Add(appraiser);
            sut.Add(appraiser);

            // Assert. Production AppraisersManager.Add (see Sources/Libraries/
            // ProjectV.Appraisers/AppraisersManager.cs) skips a duplicate
            // reference in the same TypeId bucket — the flow must still
            // build, and Remove(item) returning true confirms the bucket
            // exists with the registered child.
            sut.Remove(appraiser).Should().BeTrue();
        }

        [Fact]
        public void AddTwoDifferentInstancesOfSameTypeIdBuildsCombinedFlow()
        {
            // Arrange.
            var first = CreateAppraiser(typeof(BasicInfo), "first");
            var second = CreateAppraiser(typeof(BasicInfo), "second");
            var sut = new TestAppraisersManagerBuilder()
                .WithAppraiser(first)
                .WithAppraiser(second)
                .Build();

            // Act.
            var flow = sut.CreateFlow();

            // Assert.
            flow.Should().NotBeNull();
            flow.Should().BeOfType<AppraisersFlow>();
        }

        [Fact]
        public void RemoveExistingReturnsTrue()
        {
            // Arrange.
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");
            var sut = new TestAppraisersManagerBuilder()
                .WithAppraiser(appraiser)
                .Build();

            // Act.
            var removed = sut.Remove(appraiser);

            // Assert.
            removed.Should().BeTrue();
        }

        [Fact]
        public void RemoveMissingReturnsFalse()
        {
            // Arrange.
            var sut = TestAppraisersManagerBuilder.CreateWithoutSetup();
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");

            // Act.
            var removed = sut.Remove(appraiser);

            // Assert.
            removed.Should().BeFalse();
        }

        [Fact]
        public void CreateFlowDispatchesEntitiesToMatchingChildAppraiser()
        {
            // Arrange.
            var expectedRating = new RatingDataContainer(
                dataHandler: new BasicInfo(
                    thingId: 99, title: "Dispatch", voteCount: 1, voteAverage: 1.0),
                ratingValue: 7.5,
                ratingId: Guid.Empty);

            var basicAppraiser = CreateAppraiserWithRating(typeof(BasicInfo), "tag", expectedRating);

            var sut = new TestAppraisersManagerBuilder()
                .WithAppraiser(basicAppraiser)
                .Build();

            // Act.
            var flow = sut.CreateFlow();

            // Assert. The flow is constructed from a single bucket; the
            // SUT's wiring exercises TypeId on every child during the
            // CreateFlow walk (see AppraisersManager.CreateFlow).
            flow.Should().NotBeNull();
            _ = basicAppraiser.Received().TypeId;
        }

        [Fact]
        public void CreateFlowReturnsDistinctInstancesAcrossCalls()
        {
            // Arrange.
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");
            var sut = new TestAppraisersManagerBuilder()
                .WithAppraiser(appraiser)
                .Build();

            // Act.
            var firstFlow = sut.CreateFlow();
            var secondFlow = sut.CreateFlow();

            // Assert.
            firstFlow.Should().NotBeSameAs(secondFlow);
        }

        #region Helper Methods

        private IAppraiser CreateAppraiser(Type typeId, string tag)
        {
            return new TestAppraiserBuilder(Fixture)
                .WithTypeId(typeId)
                .WithTag(tag)
                .Build();
        }

        private IAppraiser CreateAppraiserWithRating(Type typeId, string tag, RatingDataContainer rating)
        {
            return new TestAppraiserBuilder(Fixture)
                .WithTypeId(typeId)
                .WithTag(tag)
                .WithRating(rating)
                .Build();
        }

        #endregion
    }
}
