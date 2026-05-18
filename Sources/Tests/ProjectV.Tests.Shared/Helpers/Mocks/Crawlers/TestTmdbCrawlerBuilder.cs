using Acolyte.Assertions;
using ProjectV.Crawlers;
using ProjectV.Models.Data;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Crawlers
{
    /// <summary>
    /// Builder for <see cref="ICrawler" /> test doubles representing a TMDb
    /// crawler (Decision D-33). Wraps an
    /// <see cref="NSubstitute.Substitute" /> for <see cref="ICrawler" /> with
    /// canned <see cref="BasicInfo" /> responses produced via an async
    /// enumerable to match the production
    /// <see cref="ICrawler.GetResponse(string, bool)" /> shape (it returns
    /// <see cref="IAsyncEnumerable{T}" />, not <see cref="Task{T}" />).
    /// </summary>
    /// <remarks>
    /// Sibling to <see cref="TestOmdbCrawlerBuilder" /> /
    /// <see cref="TestSteamCrawlerBuilder" />. Each ships its own builder so
    /// downstream test plans can wire crawler-specific
    /// <see cref="ICrawler.Tag" /> / <see cref="ICrawler.TypeId" /> defaults
    /// without re-writing the same boilerplate.
    /// </remarks>
    public sealed class TestTmdbCrawlerBuilder
    {
        /// <summary>
        /// Default tag value returned by the substitute. Mirrors
        /// <c>nameof(TmdbCrawler)</c> from the production class.
        /// </summary>
        public const string DefaultTag = "TmdbCrawler";

        private readonly List<BasicInfo> _responses = new List<BasicInfo>();
        private string _tag = DefaultTag;
        private Type _typeId = typeof(BasicInfo);
        private Exception? _throwOnGetResponse;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestTmdbCrawlerBuilder" /> class. No responses are
        /// configured until <see cref="WithResponse" /> /
        /// <see cref="WithResponses" /> is called.
        /// </summary>
        public TestTmdbCrawlerBuilder()
        {
        }

        /// <summary>
        /// Convenience factory returning a bare <see cref="ICrawler" />
        /// substitute with the <see cref="DefaultTag" />, the default
        /// <c>typeof(BasicInfo)</c> type id, and an empty response stream.
        /// </summary>
        public static ICrawler CreateWithoutSetup()
        {
            return new TestTmdbCrawlerBuilder().Build();
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
        /// Registers a batch of <see cref="BasicInfo" /> responses at once.
        /// </summary>
        /// <param name="responses">
        /// Responses to yield. Must not be <c>null</c>; null elements are
        /// rejected.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestTmdbCrawlerBuilder WithResponses(IReadOnlyList<BasicInfo> responses)
        {
            responses.ThrowIfNull(nameof(responses));

            foreach (BasicInfo response in responses)
            {
                response.ThrowIfNull(nameof(responses));
                _responses.Add(response);
            }

            return this;
        }

        /// <summary>
        /// Overrides the <see cref="ICrawler.Tag" /> value returned by the
        /// substitute. Defaults to <see cref="DefaultTag" />.
        /// </summary>
        /// <param name="tag">Tag value. Must not be <c>null</c>/whitespace.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestTmdbCrawlerBuilder WithTag(string tag)
        {
            tag.ThrowIfNullOrWhiteSpace(nameof(tag));

            _tag = tag;
            return this;
        }

        /// <summary>
        /// Overrides the <see cref="ICrawler.TypeId" /> value returned by the
        /// substitute. Defaults to <c>typeof(BasicInfo)</c>.
        /// </summary>
        /// <param name="typeId">Type id. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestTmdbCrawlerBuilder WithTypeId(Type typeId)
        {
            typeId.ThrowIfNull(nameof(typeId));

            _typeId = typeId;
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
            var substitute = Substitute.For<ICrawler>();

            substitute.Tag.Returns(_tag);
            substitute.TypeId.Returns(_typeId);

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

            // Force the method to be truly asynchronous so callers cannot
            // accidentally treat the substitute as a synchronous source.
            await Task.CompletedTask;
        }
    }
}
