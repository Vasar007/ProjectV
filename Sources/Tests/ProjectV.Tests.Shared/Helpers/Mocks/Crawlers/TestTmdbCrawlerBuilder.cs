using Acolyte.Assertions;
using AutoFixture;
using ProjectV.Crawlers;
using ProjectV.Models.Data;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Crawlers
{
    /// <summary>
    /// Builder for <see cref="ICrawler" /> test doubles representing a TMDb
    /// crawler. Wraps an AutoFixture-created substitute for <see cref="ICrawler" />
    /// with canned <see cref="BasicInfo" /> responses produced via an async
    /// enumerable to match the production
    /// <see cref="ICrawler.GetResponse(string, bool)" /> shape (it returns
    /// <see cref="IAsyncEnumerable{T}" />, not <see cref="Task{T}" />).
    /// </summary>
    /// <remarks>
    /// The substitute reports fixed crawler metadata:
    /// <see cref="ICrawler.Tag" /> returns <see cref="DefaultTag" /> and
    /// <see cref="ICrawler.TypeId" /> returns <c>typeof(BasicInfo)</c>.
    /// </remarks>
    public sealed class TestTmdbCrawlerBuilder
    {
        /// <summary>
        /// Default tag value returned by the substitute. Mirrors
        /// <c>nameof(TmdbCrawler)</c> from the production class.
        /// </summary>
        public const string DefaultTag = "TmdbCrawler";

        private readonly IFixture _fixture;

        private readonly List<BasicInfo> _responses = new List<BasicInfo>();
        private Exception? _throwOnGetResponse;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestTmdbCrawlerBuilder" /> class. No responses are
        /// configured until <see cref="WithResponse" /> is called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestTmdbCrawlerBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory returning a bare <see cref="ICrawler" />
        /// substitute with the <see cref="DefaultTag" />, the default
        /// <c>typeof(BasicInfo)</c> type id, and an empty response stream.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static ICrawler CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestTmdbCrawlerBuilder(fixture).Build();
        }

        /// <summary>
        /// Registers a single <see cref="BasicInfo" /> response to be yielded
        /// for every <see cref="ICrawler.GetResponse(string, bool)" /> call.
        /// </summary>
        /// <param name="response">Response item. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestTmdbCrawlerBuilder WithResponse(BasicInfo response)
        {
            response.ThrowIfNull(nameof(response));

            _responses.Add(response);
            return this;
        }

        /// <summary>
        /// Configures the substitute to throw the supplied exception
        /// synchronously from <see cref="ICrawler.GetResponse(string, bool)" />
        /// (i.e. before the async enumerable iteration starts). Useful for
        /// exercising <c>CrawlersManager.TryGetResponse</c>'s log+rethrow
        /// behaviour.
        /// </summary>
        /// <param name="exception">Exception to throw. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestTmdbCrawlerBuilder WithThrowOnGetResponse(Exception exception)
        {
            exception.ThrowIfNull(nameof(exception));

            _throwOnGetResponse = exception;
            return this;
        }

        /// <summary>
        /// Builds the <see cref="ICrawler" /> substitute. Configured response
        /// items are yielded asynchronously from
        /// <see cref="ICrawler.GetResponse(string, bool)" />.
        /// </summary>
        public ICrawler Build()
        {
            var substitute = _fixture.Create<ICrawler>();

            substitute.Tag.Returns(DefaultTag);
            substitute.TypeId.Returns(typeof(BasicInfo));

            if (_throwOnGetResponse is not null)
            {
                var exception = _throwOnGetResponse;
                substitute
                    .GetResponse(Arg.Any<string>(), Arg.Any<bool>())
                    .Returns(_ => throw exception);
            }
            else
            {
                IReadOnlyList<BasicInfo> snapshot = _responses.ToArray();
                substitute
                    .GetResponse(Arg.Any<string>(), Arg.Any<bool>())
                    .Returns(_ => ToAsyncEnumerable(snapshot));
            }

            return substitute;
        }

        private static async IAsyncEnumerable<BasicInfo> ToAsyncEnumerable(
            IReadOnlyList<BasicInfo> items)
        {
            foreach (BasicInfo item in items)
            {
                yield return item;
            }

            // Satisfies the compiler's requirement that an async iterator
            // contains an await; the already-completed task continues
            // synchronously, so iteration itself remains synchronous.
            await Task.CompletedTask;
        }
    }
}
