using System.Net.Http;
using System.Threading;

namespace ProjectV.Tests.Shared.Helpers.Http
{
    /// <summary>
    /// Test-only <see cref="DelegatingHandler" /> that returns a deterministic
    /// <see cref="HttpResponseMessage" /> for every call, driven by a
    /// caller-supplied responder closure. Exposes a public read-only
    /// <see cref="CallCount" /> counter so tests can assert on invocation
    /// shape (e.g. retry counts, single-call expectations).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Hoisted to <c>ProjectV.Tests.Shared</c> in Plan 02-13 (Task 2 / IN-03).
    /// Previously duplicated as private nested types inside
    /// <c>ProjectV.Core.Tests.Net.CommunicationServiceClientTests</c> and
    /// <c>ProjectV.Core.Tests.Net.HttpClientPollyPolicyTests</c>; the
    /// duplicates carried identical bodies and the duplication was flagged
    /// by the Phase 2 code review (IN-03).
    /// </para>
    /// <para>
    /// We do NOT mock <see cref="HttpMessageHandler" /> via NSubstitute
    /// because NSubstitute cannot intercept protected <c>SendAsync</c>
    /// (02-RESEARCH.md "Pitfall 6: NSubstitute cannot mock protected
    /// SendAsync"). A real <see cref="DelegatingHandler" /> subclass that
    /// returns canned responses is the supported pattern.
    /// </para>
    /// <para>
    /// The handler does NOT call <c>base.SendAsync</c>; it answers from the
    /// responder closure directly. That makes it safe to use either as a
    /// primary handler (no inner handler) or wrapped — Polly's retry policy
    /// drives multiple invocations through the same primary instance, which
    /// is why <see cref="CallCount" /> exists.
    /// </para>
    /// </remarks>
    public sealed class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

        /// <summary>
        /// Number of times <see cref="SendAsync" /> has been invoked on this
        /// instance. Read-only externally; tests assert on this value to
        /// verify retry counts and single-call expectations.
        /// </summary>
        public int CallCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="FakeHttpMessageHandler" /> class.
        /// </summary>
        /// <param name="responder">
        /// Callback that turns each <see cref="HttpRequestMessage" /> into an
        /// <see cref="HttpResponseMessage" />. The handler attaches the
        /// inbound request to the produced response via
        /// <c>RequestMessage</c> before returning.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="responder" /> is <see langword="null" />.
        /// </exception>
        public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
        {
            _responder = responder ?? throw new ArgumentNullException(nameof(responder));
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            HttpResponseMessage response = _responder(request);
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }
}
