using System.Net.Http;
using Acolyte.Assertions;
using AutoFixture;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Net
{
    /// <summary>
    /// Builder for <see cref="IHttpClientFactory" /> test doubles backed by
    /// an AutoFixture-supplied <see cref="NSubstitute" /> substitute.
    /// Configures <c>CreateClient(Any)</c> to return a caller-supplied
    /// <see cref="HttpClient" /> — usually one backed by a fake message
    /// handler so the production code's outbound requests can be observed.
    /// </summary>
    public sealed class TestHttpClientFactoryBuilder
    {
        private readonly IFixture _fixture;
        private HttpClient? _httpClient;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestHttpClientFactoryBuilder" /> class. No client is
        /// configured until <see cref="WithHttpClient" /> is called.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public TestHttpClientFactoryBuilder(IFixture fixture)
        {
            _fixture = fixture.ThrowIfNull(nameof(fixture));
        }

        /// <summary>
        /// Convenience factory that returns a bare-bones
        /// <see cref="IHttpClientFactory" /> substitute with no configured
        /// <c>CreateClient</c> behavior.
        /// </summary>
        /// <param name="fixture">AutoFixture instance to create the substitute.</param>
        public static IHttpClientFactory CreateWithoutSetup(IFixture fixture)
        {
            fixture.ThrowIfNull(nameof(fixture));
            return new TestHttpClientFactoryBuilder(fixture).Build();
        }

        /// <summary>
        /// Configures the factory so every <c>CreateClient(...)</c> call
        /// returns the supplied <paramref name="httpClient" />.
        /// </summary>
        /// <param name="httpClient">
        /// The <see cref="HttpClient" /> to return. Must not be <c>null</c>.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestHttpClientFactoryBuilder WithHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient.ThrowIfNull(nameof(httpClient));
            return this;
        }

        /// <summary>
        /// Builds the <see cref="IHttpClientFactory" /> substitute. If
        /// <see cref="WithHttpClient" /> has been called, every
        /// <c>CreateClient(...)</c> call will return the configured client;
        /// otherwise the substitute returns whatever AutoFixture / NSubstitute
        /// would by default.
        /// </summary>
        public IHttpClientFactory Build()
        {
            var factory = _fixture.Create<IHttpClientFactory>();

            if (_httpClient is not null)
            {
                factory.CreateClient(Arg.Any<string>()).Returns(_httpClient);
            }

            return factory;
        }
    }
}
