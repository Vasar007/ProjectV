using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Configuration.Options;
using ProjectV.Core.DependencyInjection;
using ProjectV.Tests.Shared.Helpers.Http;
using Xunit;

namespace ProjectV.Core.Tests.Net
{
    /// <summary>
    /// Unit tests for the Polly retry policy wired by
    /// <c>AddHttpClientWithOptions</c> /
    /// <c>HttpClientBuilderExtensions.AddHttpErrorPoliciesWithOptions</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Uses an in-test <see cref="FakeHttpMessageHandler" /> (DelegatingHandler
    /// subclass) to simulate transient HTTP errors — the
    /// <c>Substitute.For&lt;HttpMessageHandler&gt;</c> anti-pattern is avoided
    /// because NSubstitute cannot mock protected methods (02-RESEARCH.md
    /// "Pitfall 6"). Production code under test:
    /// <c>services.AddHttpClient(name).AddHttpOptions(options)</c> →
    /// <c>AddTransientHttpErrorPolicy(...)</c> →
    /// <c>WaitAndRetryWithOptionsAsync(retryCount = RetryCountOnFailed,
    /// retryTimeout = RetryTimeoutOnFailed)</c>. AddTransientHttpErrorPolicy
    /// covers HTTP 5xx + 408 + network failures by default.
    /// </para>
    /// </remarks>
    [Trait("Category", "Unit")]
    public sealed class HttpClientPollyPolicyTests
    {
        private const string TestClientName = "test-polly-client";

        public HttpClientPollyPolicyTests()
        {
        }

        [Fact]
        public async Task AddHttpClientWithOptions_With503TransientThenOk_RetriesUntilSuccess()
        {
            // Arrange.
            // The Polly retry policy is configured with RetryCountOnFailed = 3 and a
            // 1 ms back-off (overridden here so the test finishes in well under a
            // second). Queue: [503, 503, 503, 200] → expect 4 handler invocations.
            var responses = new Queue<HttpStatusCode>(new[]
            {
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.OK,
            });
            var handler = new FakeHttpMessageHandler(_ =>
            {
                HttpStatusCode statusCode = responses.Count > 0
                    ? responses.Dequeue()
                    : HttpStatusCode.OK;
                return new HttpResponseMessage(statusCode);
            });

            HttpClient client = BuildHttpClientWithRetryPolicy(
                retryCountOnFailed: 3,
                retryTimeoutOnFailed: TimeSpan.FromMilliseconds(1),
                primaryHandler: handler
            );

            // Act.
            using HttpResponseMessage response = await client.GetAsync("/probe");

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            handler.CallCount.Should().Be(4,
                "the policy retries 3 times after the initial 503 then succeeds");
        }

        [Fact]
        public async Task AddHttpClientWithOptions_WithAlways503_StopsAfterRetryCount()
        {
            // Arrange.
            // Every response is 503; the policy retries RetryCountOnFailed = 2
            // times after the initial attempt, then surfaces the final 503 to the
            // caller. Expect 1 + 2 = 3 handler invocations.
            var handler = new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));

            HttpClient client = BuildHttpClientWithRetryPolicy(
                retryCountOnFailed: 2,
                retryTimeoutOnFailed: TimeSpan.FromMilliseconds(1),
                primaryHandler: handler
            );

            // Act.
            using HttpResponseMessage response = await client.GetAsync("/probe");

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            handler.CallCount.Should().Be(3,
                "initial attempt + 2 retries before giving up");
        }

        [Fact]
        public async Task AddHttpClientWithOptions_With200OnFirstCall_DoesNotRetry()
        {
            // Arrange.
            var handler = new FakeHttpMessageHandler(_ =>
                new HttpResponseMessage(HttpStatusCode.OK));

            HttpClient client = BuildHttpClientWithRetryPolicy(
                retryCountOnFailed: 3,
                retryTimeoutOnFailed: TimeSpan.FromMilliseconds(1),
                primaryHandler: handler
            );

            // Act.
            using HttpResponseMessage response = await client.GetAsync("/probe");

            // Assert.
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            handler.CallCount.Should().Be(1,
                "no retry when the first response is a success status code");
        }

        /// <summary>
        /// Builds a <see cref="HttpClient" /> by registering one via
        /// <c>AddHttpClientWithOptions</c> on a fresh
        /// <see cref="ServiceCollection" />, then overriding the primary
        /// HTTP message handler with the supplied test handler so the
        /// Polly retry policy is the only production behavior under test.
        /// </summary>
        private static HttpClient BuildHttpClientWithRetryPolicy(
            int retryCountOnFailed,
            TimeSpan retryTimeoutOnFailed,
            HttpMessageHandler primaryHandler)
        {
            var options = new HttpClientOptions
            {
                HttpClientDefaultName = TestClientName,
                RetryCountOnFailed = retryCountOnFailed,
                RetryTimeoutOnFailed = retryTimeoutOnFailed,
                RetryCountOnAuth = 0,
                RetryTimeoutOnAuth = TimeSpan.FromMilliseconds(1),
                ShouldDisposeHttpClient = false
            };

            var services = new ServiceCollection();
            services.AddHttpClientWithOptions(options)
                .ConfigurePrimaryHttpMessageHandler(() => primaryHandler);

            ServiceProvider provider = services.BuildServiceProvider();
            IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
            HttpClient client = factory.CreateClient(TestClientName);
            client.BaseAddress = new Uri("http://localhost:8000/");
            return client;
        }

    }
}
