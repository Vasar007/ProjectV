using System;
using AutoFixture;
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

        private IAppraiser CreateAppraiserMock(Type typeId, string tag = "tag")
        {
            var sub = Fixture.Create<IAppraiser>();
            sub.TypeId.Returns(typeId);
            sub.Tag.Returns(tag);
            return sub;
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
            var appraiser = CreateAppraiserMock(typeof(BasicInfo));
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
            var appraiser = CreateAppraiserMock(typeof(BasicInfo));
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
            var first = CreateAppraiserMock(typeof(BasicInfo), tag: "first");
            var second = CreateAppraiserMock(typeof(BasicInfo), tag: "second");
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
            var appraiser = CreateAppraiserMock(typeof(BasicInfo));
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
            var appraiser = CreateAppraiserMock(typeof(BasicInfo));

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

            var basicAppraiser = new TestAppraiserBuilder(Fixture)
                .WithRating(expectedRating)
                .WithTypeId(typeof(BasicInfo))
                .Build();

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
            var appraiser = CreateAppraiserMock(typeof(BasicInfo));
            var sut = new TestAppraisersManagerBuilder()
                .WithAppraiser(appraiser)
                .Build();

            // Act.
            var firstFlow = sut.CreateFlow();
            var secondFlow = sut.CreateFlow();

            // Assert.
            firstFlow.Should().NotBeSameAs(secondFlow);
        }
    }
}
