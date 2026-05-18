using Acolyte.Assertions;
using ProjectV.Crawlers;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Managers
{
    /// <summary>
    /// Builder for real <see cref="CrawlersManager" /> instances populated
    /// with <see cref="NSubstitute" /> child <see cref="ICrawler" /> doubles
    /// (Decision D-33 fallback). <see cref="CrawlersManager" /> is
    /// <c>sealed</c> without a substitution-friendly interface seam, so this
    /// builder returns a real manager populated through its public
    /// <see cref="CrawlersManager.Add(ICrawler)" /> API.
    /// </summary>
    public sealed class TestCrawlersManagerBuilder
    {
        private readonly List<ICrawler> _crawlers = new List<ICrawler>();
        private bool _outputResults;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestCrawlersManagerBuilder" /> class. No crawlers are
        /// registered until one of the <c>With*</c> methods is called.
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
        /// Sets the <c>outputResults</c> flag on the resulting
        /// <see cref="CrawlersManager" />.
        /// </summary>
        /// <param name="outputResults">
        /// Whether the manager should propagate <c>outputResults=true</c> to
        /// every child crawler. Defaults to <c>false</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCrawlersManagerBuilder WithOutputResults(bool outputResults)
        {
            _outputResults = outputResults;
            return this;
        }

        /// <summary>
        /// Registers an <see cref="ICrawler" /> child to be added to the
        /// resulting <see cref="CrawlersManager" />.
        /// </summary>
        /// <param name="crawler">
        /// Crawler substitute to register. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCrawlersManagerBuilder WithCrawler(ICrawler crawler)
        {
            crawler.ThrowIfNull(nameof(crawler));

            _crawlers.Add(crawler);
            return this;
        }

        /// <summary>
        /// Registers a batch of <see cref="ICrawler" /> children at once.
        /// </summary>
        /// <param name="crawlers">
        /// Crawler substitutes to register. Must not be <c>null</c>; null
        /// elements are rejected.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestCrawlersManagerBuilder WithCrawlers(IReadOnlyList<ICrawler> crawlers)
        {
            crawlers.ThrowIfNull(nameof(crawlers));

            foreach (ICrawler crawler in crawlers)
            {
                crawler.ThrowIfNull(nameof(crawlers));
                _crawlers.Add(crawler);
            }

            return this;
        }

        /// <summary>
        /// Builds the <see cref="CrawlersManager" /> instance pre-populated
        /// with the registered children.
        /// </summary>
        public CrawlersManager Build()
        {
            var manager = new CrawlersManager(_outputResults);
            foreach (ICrawler crawler in _crawlers)
            {
                manager.Add(crawler);
            }

            return manager;
        }
    }
}
