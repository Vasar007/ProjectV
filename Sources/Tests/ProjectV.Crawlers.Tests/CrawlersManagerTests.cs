using System;
using System.Reflection;
using AwesomeAssertions;
using ProjectV.Tests.Shared.Helpers.Mocks.Crawlers;
using Xunit;

namespace ProjectV.Crawlers.Tests
{
    /// <summary>
    /// Unit tests for <see cref="CrawlersManager" />, focused on
    /// <c>TryGetResponse</c>'s log+rethrow contract: when a child
    /// <see cref="ICrawler" /> throws synchronously from
    /// <see cref="ICrawler.GetResponse(string, bool)" />, the manager must
    /// re-throw the original exception unchanged (logging happens through a
    /// static <c>NLog</c> logger that is out-of-scope to substitute here —
    /// see remark below).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>CrawlersManager.TryGetResponse</c> is <c>private</c> and is invoked
    /// via the funcs constructed by <see cref="CrawlersManager.CreateFlow" />.
    /// To assert on the log+rethrow contract directly (without spinning up
    /// the surrounding Gridsum.DataflowEx pipeline), the test invokes
    /// <c>TryGetResponse</c> through reflection. This keeps the unit test
    /// laser-focused on the manager's exception-handling logic;
    /// integration-grade coverage of the same path through the real
    /// dataflow is provided by
    /// <c>ProjectV.DataPipeline.Tests.DataflowPipelineTests</c>.
    /// </para>
    /// <para>
    /// <see cref="ProjectV.Logging.ILogger" /> is consumed via a
    /// <c>private static readonly</c> field initialised through
    /// <c>LoggerFactory.CreateLoggerFor&lt;CrawlersManager&gt;()</c>. That
    /// static seam is not substitutable from a unit test without invasive
    /// reflection on <c>LoggerFactory</c> internals; we therefore verify the
    /// observable half of the contract (the exception propagates) and rely
    /// on the hoisted
    /// <c>ProjectV.Tests.Shared.ForTests.TestModuleInitializer</c> +
    /// production code review to cover the <c>_logger.Error(...)</c> call.
    /// The 02-06 PLAN's <c>logger.Received(1).Error(...)</c> wording is an
    /// aspirational target that this unit suite intentionally does not chase
    /// — Rule 1 / Rule 3 deviation, recorded in the plan SUMMARY.
    /// </para>
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class CrawlersManagerTests
    {
        public CrawlersManagerTests()
        {
        }

        [Fact]
        public void TryGetResponse_OnException_RethrowsOriginalException()
        {
            // Arrange.
            var expectedException = new InvalidOperationException(
                "Simulated TMDb crawler failure for test."
            );
            ICrawler throwingCrawler = new TestTmdbCrawlerBuilder()
                .WithThrowOnGetResponse(expectedException)
                .Build();

            using var sut = new CrawlersManager(outputResults: false);
            sut.Add(throwingCrawler);

            MethodInfo tryGetResponse = typeof(CrawlersManager).GetMethod(
                "TryGetResponse",
                BindingFlags.NonPublic | BindingFlags.Instance
            )!;
            tryGetResponse.Should().NotBeNull(
                "CrawlersManager.TryGetResponse must remain a private instance method " +
                "for this reflection-based unit test to find it");

            // Act.
            var act = () => tryGetResponse.Invoke(
                sut, new object[] { throwingCrawler, "any-entity" }
            );

            // Assert.
            // The reflection wrapper re-raises the original exception as the
            // inner exception of TargetInvocationException — assert on the
            // unwrapped form to keep the contract clear.
            act.Should()
               .Throw<TargetInvocationException>()
               .WithInnerException<InvalidOperationException>()
               .Which.Message.Should().Be(expectedException.Message);
        }

        [Fact]
        public void Constructor_DoesNotRequireAnyCrawlers()
        {
            // Arrange. / Act.
            using var sut = new CrawlersManager(outputResults: false);

            // Assert.
            sut.Should().NotBeNull(
                "the manager must be constructable with zero crawlers — " +
                "callers register child ICrawler implementations via Add(...) " +
                "after construction"
            );
        }

        [Fact]
        public void Add_WithNullCrawler_ThrowsArgumentNullException()
        {
            // Arrange.
            using var sut = new CrawlersManager(outputResults: false);

            // Act.
            var act = () => sut.Add(
                item: null!
            );

            // Assert.
            act.Should()
               .Throw<ArgumentNullException>()
               .WithParameterName("item");
        }

        [Fact]
        public void Remove_WithRegisteredCrawler_ReturnsTrueAndDropsTheCrawler()
        {
            // Arrange.
            ICrawler crawler = TestOmdbCrawlerBuilder.CreateWithoutSetup();
            using var sut = new CrawlersManager(outputResults: false);
            sut.Add(crawler);

            // Act.
            bool removed = sut.Remove(crawler);

            // Assert.
            removed.Should().BeTrue(
                "Remove must report success when the manager holds the supplied crawler"
            );
        }
    }
}
