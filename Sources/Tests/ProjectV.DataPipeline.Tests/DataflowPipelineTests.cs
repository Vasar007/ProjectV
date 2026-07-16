using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using NSubstitute;
using ProjectV.Appraisers;
using ProjectV.Crawlers;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;
using ProjectV.Tests.Shared.ForTests;
using ProjectV.Tests.Shared.Helpers.Generators.Models;
using ProjectV.Tests.Shared.Helpers.Mocks.Appraisers;
using ProjectV.Tests.Shared.Helpers.Mocks.Crawlers;
using Xunit;

namespace ProjectV.DataPipeline.Tests
{
    /// <summary>
    /// Integration tests for <see cref="DataflowPipeline" />, focused on the
    /// end-to-end <see cref="DataflowPipeline.Execute(string)" /> path: a
    /// real Gridsum.DataflowEx host wired with
    /// <see cref="InputtersFlow" /> →
    /// <see cref="CrawlersFlow" /> →
    /// <see cref="AppraisersFlow" /> →
    /// <see cref="OutputtersFlow" />, with NSubstitute
    /// <see cref="ICrawler" /> + <see cref="IAppraiser" /> leaves at the
    /// stage boundaries.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Tagged <c>Integration</c> (not <c>Unit</c>) because the test
    /// exercises real TPL Dataflow blocks and the real
    /// <c>Gridsum.DataflowEx</c> dataflow host. Mocks are confined to the
    /// leaf <see cref="ICrawler" /> + <see cref="IAppraiser" /> NSubstitute
    /// substitutes — every block, every link, every completion-propagation
    /// rule between stages is real.
    /// </para>
    /// <para>
    /// The pipeline composition mirrors the production
    /// <c>Shell.ConstructPipeline</c> ordering
    /// (<c>inputtersFlow.LinkTo(crawlersFlow)</c> →
    /// <c>crawlersFlow.LinkTo(appraisersFlow)</c> →
    /// <c>appraisersFlow.LinkTo(outputtersFlow)</c>). The test never
    /// touches the production <c>Shell</c> / <c>InputManager</c> /
    /// <c>CrawlersManager</c> / <c>AppraisersManager</c> /
    /// <c>OutputManager</c> wiring — those are covered by their own
    /// suites — so a regression in pipeline plumbing surfaces here
    /// independent of manager-level concerns.
    /// </para>
    /// </remarks>
    [Trait("Category", "Integration")]
    public sealed class DataflowPipelineTests : BaseMockTest
    {
        private readonly BasicInfoGenerator _generator;

        public DataflowPipelineTests()
        {
            _generator = BasicInfoGenerator.Instance;
        }

        [Fact]
        public async Task Execute_WithStubCrawlersAndAppraisers_ProducesExpectedOutput()
        {
            // Arrange.
            // 1. Input entity name that survives the InputtersFlow filter
            //    (Length > MinWordLength = 2 + dedup).
            const string entityName = "Inception";

            // 2. The single inputter echoes the storage-name input back as
            //    the entity name so the crawler stage gets a known string.
            var inputters = new[]
            {
                new Func<string, IEnumerable<string>>(_ => new[] { entityName }),
            };
            var inputtersFlow = new InputtersFlow(inputters);

            // 3. NSubstitute ICrawler that yields one BasicInfo for any input.
            BasicInfo expectedBasicInfo = CreateBasicInfo(
                thingId: 42,
                title: entityName,
                voteCount: 10_000,
                voteAverage: 8.7
            );
            ICrawler crawlerSubstitute = CreateCrawler(expectedBasicInfo);

            var crawlerFuncs = new[]
            {
                new Func<string, IAsyncEnumerable<BasicInfo>>(
                    name => crawlerSubstitute.GetResponse(name, outputResults: false)
                ),
            };
            var crawlersFlow = new CrawlersFlow(crawlerFuncs);

            // 4. NSubstitute IAppraiser that returns a fixed rating for any
            //    BasicInfo. The AppraisersFlow Funcotype uses DataType =
            //    typeof(BasicInfo) so all BasicInfo inputs match the
            //    IsAssignableFrom(...) filter.
            RatingDataContainer expectedRating = CreateRating(
                dataHandler: expectedBasicInfo,
                ratingValue: 9.1,
                ratingId: Guid.NewGuid()
            );
            var appraiserSubstitute = CreateAppraiser(expectedRating);

            var appraisersFuncs = new[]
            {
                new Funcotype(
                    func: info => appraiserSubstitute.GetRatings(info, outputResults: false),
                    dataType: typeof(BasicInfo)
                ),
            };
            var appraisersFlow = new AppraisersFlow(appraisersFuncs);

            // 5. OutputtersFlow with a capture-sink action that appends every
            //    received rating to a concurrent collection — assertions
            //    happen on that collection after the pipeline completes.
            var collected = new ConcurrentBag<RatingDataContainer>();
            var outputterFuncs = new[]
            {
                new Action<RatingDataContainer>(collected.Add),
            };
            var outputtersFlow = new OutputtersFlow(outputterFuncs);

            // 6. Wire the four stages in the same order as
            //    Shell.ConstructPipeline (Sources/Libraries/ProjectV.Core/Shell.cs).
            inputtersFlow.LinkTo(crawlersFlow);
            crawlersFlow.LinkTo(appraisersFlow);
            appraisersFlow.LinkTo(outputtersFlow);

            var sut = new DataflowPipeline(inputtersFlow, outputtersFlow);

            // Act.
            // The production `DataflowPipeline.Execute(string)` uses
            // `InputtersFlow.ProcessAsync(new[] { input })` (the single-arg
            // overload that leaves the flow open for more input). With a
            // finite test input that single-arg overload never signals
            // upstream completion, so `OutputtersFlow.CompletionTask` would
            // block forever — the same Gridsum.DataflowEx empty/terminal
            // pipeline deadlock that prevents Shell.Run from being covered
            // by a unit test against a finite input.
            //
            // To exercise the SAME end-to-end wiring without hanging, this
            // test reproduces Execute's logical contract via the two-arg
            // ProcessAsync overload (`completeFlowOnFinish: true`) and then
            // awaits OutputtersFlow.CompletionTask. The two-line body is
            // the only "test-flavoured" deviation from the production
            // Execute method — the pipeline plumbing under test is
            // identical.
            await sut.InputtersFlow.ProcessAsync(
                new[] { "any-storage-name" },
                completeFlowOnFinish: true
            );
            await sut.OutputtersFlow.CompletionTask;

            // Assert.
            // The pipeline emitted exactly one RatingDataContainer; the
            // outputter sink captured it, and it carries the SAME values
            // the substitute appraiser was configured to return.
            collected.Should().HaveCount(
                1,
                "the pipeline must deliver exactly one rating when given one " +
                "entity name through one crawler / one appraiser / one outputter");

            RatingDataContainer rating = collected.Single();
            rating.DataHandler.Should().BeSameAs(
                expectedBasicInfo,
                "the BasicInfo emitted by the crawler must flow unchanged through " +
                "the AppraisersFlow into the OutputtersFlow");
            rating.RatingValue.Should().Be(
                expectedRating.RatingValue,
                "the rating value returned by the IAppraiser substitute must " +
                "round-trip through the dataflow without mutation");
            rating.RatingId.Should().Be(
                expectedRating.RatingId,
                "the rating identifier must round-trip through the dataflow " +
                "without mutation");

            // The substitute crawler must have been invoked exactly once.
            crawlerSubstitute.Received(1).GetResponse(
                Arg.Any<string>(), Arg.Any<bool>()
            );
            // The substitute appraiser must have been invoked exactly once
            // (one BasicInfo flowed through the crawler stage → one rating).
            appraiserSubstitute.Received(1).GetRatings(
                Arg.Any<BasicInfo>(), Arg.Any<bool>()
            );
        }

        [Fact]
        public void Constructor_OnAllArgumentsProvided_CreatesInstance()
        {
            // Arrange.
            var inputtersFlow = CreateInputtersFlow();
            var outputtersFlow = CreateOutputtersFlow();

            // Act.
            var act = () => CreateDataflowPipelineWithAllArguments(
                inputtersFlow, outputtersFlow
            );

            // Assert.
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenNullValueProvided()
        {
            // Arrange.
            var inputtersFlow = CreateInputtersFlow();
            var outputtersFlow = CreateOutputtersFlow();

            var actions = new List<(Action act, string paramName)>
            {
                (() => CreateDataflowPipelineWithAllArguments(null!, outputtersFlow),
                    "inputtersFlow"),
                (() => CreateDataflowPipelineWithAllArguments(inputtersFlow, null!),
                    "outputtersFlow"),
            };

            // Act. / Assert.
            foreach ((Action act, string paramName) in actions)
            {
                act.Should()
                   .Throw<ArgumentNullException>()
                   .WithParameterName(paramName);
            }
        }

        #region Helper Methods

        /// <summary>
        /// Creates a <see cref="DataflowPipeline" /> from every constructor
        /// dependency explicitly. Constructor tests arrange valid (empty)
        /// flows via the per-dependency <c>Create*</c> helpers and pass
        /// exactly one dependency as <c>null!</c> to exercise the matching
        /// null-guard.
        /// </summary>
        private static DataflowPipeline CreateDataflowPipelineWithAllArguments(
            InputtersFlow inputtersFlow,
            OutputtersFlow outputtersFlow)
        {
            return new DataflowPipeline(
                inputtersFlow: inputtersFlow,
                outputtersFlow: outputtersFlow
            );
        }

        /// <summary>
        /// Creates an <see cref="InputtersFlow" /> with no inputters —
        /// sufficient for constructor-guard tests that never run the flow.
        /// </summary>
        private static InputtersFlow CreateInputtersFlow()
        {
            return new InputtersFlow(
                Array.Empty<Func<string, IEnumerable<string>>>()
            );
        }

        /// <summary>
        /// Creates an <see cref="OutputtersFlow" /> with no outputters —
        /// sufficient for constructor-guard tests that never run the flow.
        /// </summary>
        private static OutputtersFlow CreateOutputtersFlow()
        {
            return new OutputtersFlow(
                Array.Empty<Action<RatingDataContainer>>()
            );
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

        private ICrawler CreateCrawler(BasicInfo response)
        {
            return new TestTmdbCrawlerBuilder(Fixture)
                .WithResponse(response)
                .Build();
        }

        private IAppraiser CreateAppraiser(RatingDataContainer rating)
        {
            return new TestAppraiserBuilder(Fixture)
                .WithRating(rating)
                .Build();
        }

        #endregion
    }
}
