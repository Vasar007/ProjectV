using ProjectV.Crawlers;

namespace ProjectV.Tests.Shared.Helpers.Stubs.Managers
{
    /// <summary>
    /// Builder for real (empty) <see cref="CrawlersManager" /> instances.
    /// <see cref="CrawlersManager" /> is <c>sealed</c> without a
    /// substitution-friendly interface seam, so this
    /// builder returns a real manager.
    /// </summary>
    public sealed class TestCrawlersManagerBuilder
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestCrawlersManagerBuilder" /> class.
        /// </summary>
        public TestCrawlersManagerBuilder()
        {
        }

        /// <summary>
        /// Convenience factory that returns an empty
        /// <see cref="CrawlersManager" /> with no children registered and
        /// <c>outputResults</c> set to <c>false</c>.
        /// </summary>
        public static CrawlersManager CreateWithoutSetup()
        {
            return new TestCrawlersManagerBuilder().Build();
        }

        /// <summary>
        /// Builds an empty <see cref="CrawlersManager" /> instance with
        /// <c>outputResults</c> set to <c>false</c>.
        /// </summary>
        public CrawlersManager Build()
        {
            return new CrawlersManager(outputResults: false);
        }
    }
}
