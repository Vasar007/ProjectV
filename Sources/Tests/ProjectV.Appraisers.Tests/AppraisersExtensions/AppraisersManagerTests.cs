using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Acolyte.Common.Monads;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.DataPipeline;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
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
        private readonly BasicInfoGenerator _generator;

        public AppraisersManagerTests()
        {
            _generator = BasicInfoGenerator.Instance;
        }

        [Fact]
        public void CreateWithoutSetupReturnsEmptyManager()
        {
            // Arrange.
            var sut = CreateAppraisersManager();

            // Act.
            var flow = sut.CreateFlow();

            // Assert. An empty manager produces a non-null but childless flow.
            sut.Should().NotBeNull();
            flow.Should().NotBeNull();
            flow.Should().BeOfType<AppraisersFlow>();
        }

        [Fact]
        public void AddThrowsForNullAppraiser()
        {
            // Arrange.
            var sut = CreateAppraisersManager();

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
            var sut = CreateAppraisersManager();

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
        public void AddThenCreateFlowReadsRegisteredAppraiserTypeId()
        {
            // Arrange.
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");
            var sut = CreateAppraisersManager(appraiser);

            // Act.
            var flow = sut.CreateFlow();

            // Assert. The flow construction reads TypeId from every child;
            // end-to-end dispatch through the flow is covered by
            // CreateFlowDispatchesEntitiesToMatchingChildAppraiser.
            _ = appraiser.Received().TypeId;
            flow.Should().NotBeNull();
        }

        [Fact]
        public async Task AddSameInstanceTwiceIsIdempotentWithinSameTypeId()
        {
            // Arrange.
            BasicInfo entity = CreateBasicInfo(
                thingId: 7, title: "Idempotent", voteCount: 1, voteAverage: 1.0);
            RatingDataContainer rating = CreateRating(
                dataHandler: entity,
                ratingValue: 4.2,
                ratingId: Guid.Empty);
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag", rating);
            var sut = CreateAppraisersManager();

            // Act. Production AppraisersManager.Add skips a duplicate
            // reference in the same TypeId bucket, so the flow must contain
            // exactly one child appraiser.
            sut.Add(appraiser);
            sut.Add(appraiser);
            var flow = sut.CreateFlow();

            RatingDataContainer emitted = await DriveFlowWithSingleEntityAsync(flow, entity);

            // Assert. A duplicate registration would produce a second child
            // flow and therefore a second GetRatings call for the same
            // entity — exactly one call proves the Add was idempotent.
            emitted.Should().BeSameAs(rating);
            appraiser.Received(1).GetRatings(Arg.Any<BasicInfo>(), Arg.Any<bool>());
        }

        [Fact]
        public void AddTwoDifferentInstancesOfSameTypeIdBuildsCombinedFlow()
        {
            // Arrange.
            var first = CreateAppraiser(typeof(BasicInfo), "first");
            var second = CreateAppraiser(typeof(BasicInfo), "second");
            var sut = CreateAppraisersManager(first, second);

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
            var sut = CreateAppraisersManager(appraiser);

            // Act.
            var removed = sut.Remove(appraiser);

            // Assert.
            removed.Should().BeTrue();
        }

        [Fact]
        public void RemoveMissingReturnsFalse()
        {
            // Arrange.
            var sut = CreateAppraisersManager();
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");

            // Act.
            var removed = sut.Remove(appraiser);

            // Assert.
            removed.Should().BeFalse();
        }

        [Fact]
        public async Task CreateFlowDispatchesEntitiesToMatchingChildAppraiser()
        {
            // Arrange.
            BasicInfo entity = CreateBasicInfo(
                thingId: 99, title: "Dispatch", voteCount: 1, voteAverage: 1.0);
            RatingDataContainer expectedRating = CreateRating(
                dataHandler: entity,
                ratingValue: 7.5,
                ratingId: Guid.Empty);

            var basicAppraiser = CreateAppraiser(typeof(BasicInfo), "tag", expectedRating);
            var sut = CreateAppraisersManager(basicAppraiser);
            var flow = sut.CreateFlow();

            // Act. Push one entity through the constructed flow and capture
            // what the appraiser stage emits.
            RatingDataContainer emitted = await DriveFlowWithSingleEntityAsync(flow, entity);

            // Assert. The entity reached the matching child appraiser and its
            // rating flowed out of the appraisers stage unchanged.
            emitted.Should().BeSameAs(expectedRating);
            basicAppraiser.Received(1).GetRatings(Arg.Any<BasicInfo>(), Arg.Any<bool>());
        }

        [Fact]
        public void CreateFlowReturnsDistinctInstancesAcrossCalls()
        {
            // Arrange.
            var appraiser = CreateAppraiser(typeof(BasicInfo), "tag");
            var sut = CreateAppraisersManager(appraiser);

            // Act.
            var firstFlow = sut.CreateFlow();
            var secondFlow = sut.CreateFlow();

            // Assert.
            firstFlow.Should().NotBeSameAs(secondFlow);
        }

        #region Helper Methods

        /// <summary>
        /// Posts <paramref name="entity" /> into the flow's input block,
        /// waits (with a bounded timeout) for the first emitted rating, and
        /// returns it. The flow is deliberately NOT completed — Gridsum's
        /// completion semantics deadlock when a predicated link rejects an
        /// item, so the helper observes the emission directly instead of
        /// awaiting CompletionTask. A short grace delay after the first
        /// emission lets an (unexpected) duplicate child appraiser fire so
        /// call-count assertions in the caller stay reliable.
        /// </summary>
        private static async Task<RatingDataContainer> DriveFlowWithSingleEntityAsync(
            AppraisersFlow flow, BasicInfo entity)
        {
            var firstEmission = new TaskCompletionSource<RatingDataContainer>(
                TaskCreationOptions.RunContinuationsAsynchronously);
            var sink = new ActionBlock<RatingDataContainer>(
                rating => firstEmission.TrySetResult(rating));
            flow.OutputBlock.LinkTo(sink);

            bool accepted = flow.InputBlock.Post(entity);
            accepted.Should().BeTrue(
                "the appraisers flow input block must accept a matching entity");

            Task completed = await Task.WhenAny(
                firstEmission.Task, Task.Delay(TimeSpan.FromSeconds(15)));
            completed.Should().BeSameAs(
                firstEmission.Task,
                "the flow must emit a rating for a matching entity within the timeout");

            // Grace period so a duplicate child (if one were wired) would
            // also have invoked GetRatings before the caller asserts counts.
            await Task.Delay(TimeSpan.FromMilliseconds(250));

            return await firstEmission.Task;
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

        /// <summary>
        /// Creates a <see cref="RatingDataContainer" /> with explicit values.
        /// No generator exists for the type yet, so the helper wraps the
        /// constructor to keep test bodies free of inline model creation.
        /// </summary>
        private static RatingDataContainer CreateRating(
            BasicInfo dataHandler, double ratingValue, Guid ratingId)
        {
            return new RatingDataContainer(
                dataHandler: dataHandler,
                ratingValue: ratingValue,
                ratingId: ratingId);
        }

        private AppraisersManager CreateAppraisersManager(params IAppraiser[] appraisers)
        {
            return new TestAppraisersManagerBuilder().WithAppraisers(appraisers).Build();
        }

        private IAppraiser CreateAppraiser(Type typeId, string tag, RatingDataContainer? rating = null)
        {
            return new TestAppraiserBuilder(Fixture)
                .WithTypeId(typeId)
                .WithTag(tag)
                .ApplyIf(rating is not null, x => x.WithRating(rating!))
                .Build();
        }

        #endregion
    }
}
