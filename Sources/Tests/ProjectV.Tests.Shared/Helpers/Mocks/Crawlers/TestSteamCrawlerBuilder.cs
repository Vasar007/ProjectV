using Acolyte.Assertions;
using AutoFixture;
using ProjectV.Crawlers;
using ProjectV.Models.Data;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Crawlers
{
    /// <summary>
    /// Builder for <see cref="ICrawler" /> test doubles representing a Steam
    /// crawler. Shape matches
    /// <see cref="TestTmdbCrawlerBuilder" /> verbatim; only the
    /// <see cref="DefaultTag" /> differs so downstream tests can distinguish
    /// substitutes by tag in <c>CrawlersManager</c> error messages.
    /// </summary>
    public sealed class TestSteamCrawlerBuilder
    {
        /// <summary>
        /// Default tag value returned by the substitute. Mirrors
        /// <c>nameof(SteamCrawler)</c> from the production class.
        /// </summary>
        public const string DefaultTag = "SteamCrawler";

        private readonly IFixture _fixture;

        private readonly List<BasicInfo> _responses = new List<BasicInfo>();
        private string _tag = DefaultTag;
        private Type _typeId = typeof(BasicInfo);
        private Exception? _throwOnGetResponse;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestSteamCrawlerBuilder" /> class. No responses are
        /// configured until <see cref="WithResponse" /> /
        /// <see cref="WithResponses" /> is called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestSteamCrawlerBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory returning a bare <see cref="ICrawler" />
        /// substitute with the <see cref="DefaultTag" /> and an empty
        /// response stream.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static ICrawler CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestSteamCrawlerBuilder(fixture).Build();
        }

        /// <summary>
        /// Registers a single <see cref="BasicInfo" /> response to be yielded
        /// for every <see cref="ICrawler.GetResponse(string, bool)" /> call.
        /// </summary>
        /// <param name="response">Response item. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestSteamCrawlerBuilder WithResponse(BasicInfo response)
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
        public TestSteamCrawlerBuilder WithResponses(IReadOnlyList<BasicInfo> responses)
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
        public TestSteamCrawlerBuilder WithTag(string tag)
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
        public TestSteamCrawlerBuilder WithTypeId(Type typeId)
        {
            typeId.ThrowIfNull(nameof(typeId));

            _typeId = typeId;
            return this;
        }

        /// <summary>
        /// Configures the substitute to throw the supplied exception
        /// synchronously from <see cref="ICrawler.GetResponse(string, bool)" />.
        /// </summary>
        /// <param name="exception">Exception to throw. Must not be <c>null</c>.</param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestSteamCrawlerBuilder WithThrowOnGetResponse(Exception exception)
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

            await Task.CompletedTask;
        }
    }
}
