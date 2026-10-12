using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram
{
    /// <summary>
    /// Scenario-test stub for <see cref="ITelegramBotClient" />. Accepts an
    /// optional array of <see cref="Update" /> objects in its constructor and
    /// implements the first-batch / empty-batch polling logic that the
    /// production <c>BotPolling.StartReceivingUpdatesAsync</c> path exercises
    /// via <c>ITelegramBotClient.SendRequest&lt;Update[]&gt;</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Telegram.Bot 22.x <c>ReceiveAsync</c> extension routes the polling
    /// loop through
    /// <see cref="ITelegramBotClient.SendRequest{TResponse}" /> with a
    /// <c>GetUpdatesRequest</c> whose response type is <see cref="Update" /><c>[]</c>.
    /// This stub intercepts that generic call: the first invocation returns the
    /// configured update sequence; every subsequent invocation returns an empty
    /// array. The polling loop keeps running until the host's cancellation
    /// token signals, which is the expected production behaviour.
    /// </para>
    /// <para>
    /// All other interface members return deterministic no-op / default values
    /// and are never invoked during the scenario flows exercised by the tests.
    /// </para>
    /// <para>
    /// Scenario tests compose a real dependency graph using concrete stubs
    /// instead of interface mocks, so external/leaf dependencies are replaced
    /// with deterministic stub implementations rather than NSubstitute mocks
    /// or <c>Test*Builder</c> helpers.
    /// </para>
    /// </remarks>
    public sealed class StubTelegramBotClient : ITelegramBotClient
    {
        private readonly Update[] _firstBatch;
        private readonly Update[] _emptyBatch = Array.Empty<Update>();

        // Guards the yielded flag so concurrent receive calls on the
        // same stub instance are handled safely.
        private readonly object _batchLock = new();
        private bool _yielded;

        /// <inheritdoc />
        public bool LocalBotServer => false;

        /// <inheritdoc />
        public long BotId => 0L;

        /// <inheritdoc />
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

        /// <inheritdoc />
        public IExceptionParser ExceptionsParser { get; set; } =
            new DefaultExceptionParser();

        /// <inheritdoc />
        /// <remarks>
        /// No-op add/remove: the scenario flows under test do not subscribe to
        /// or raise this event.
        /// </remarks>
        public event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest
        {
            add { }
            remove { }
        }

        /// <inheritdoc />
        /// <remarks>
        /// No-op add/remove: the scenario flows under test do not subscribe to
        /// or raise this event.
        /// </remarks>
        public event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="StubTelegramBotClient" /> class with no pre-configured
        /// updates. The stub returns an empty batch on every
        /// <see cref="SendRequest{TResponse}" /> call, which means the
        /// polling loop will keep running (with empty polls) until the
        /// cancellation token signals — useful only for tests that do not
        /// assert on update consumption.
        /// </summary>
        public StubTelegramBotClient()
            : this(Array.Empty<Update>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="StubTelegramBotClient" /> class pre-loaded with the
        /// supplied update sequence. The first
        /// <see cref="SendRequest{TResponse}" /> call for a
        /// <c>GetUpdatesRequest</c> returns the full sequence; every
        /// subsequent call returns an empty array.
        /// </summary>
        /// <param name="updates">
        /// The updates to yield on the first poll. Must not be <c>null</c>;
        /// null elements are rejected.
        /// </param>
        public StubTelegramBotClient(IReadOnlyList<Update> updates)
        {
            updates.ThrowIfNull(nameof(updates));

            var batch = new Update[updates.Count];
            for (int i = 0; i < updates.Count; i++)
            {
                updates[i].ThrowIfNull($"{nameof(updates)}[{i}]");
                batch[i] = updates[i];
            }

            _firstBatch = batch;
        }

        /// <inheritdoc />
        /// <remarks>
        /// When <typeparamref name="TResponse" /> is <see cref="Update" /><c>[]</c>
        /// this method implements the first-batch / empty-batch logic: the
        /// first call returns the configured update sequence; all subsequent
        /// calls return an empty array. All other request types return the
        /// default value for <typeparamref name="TResponse" />.
        /// </remarks>
        public Task<TResponse> SendRequest<TResponse>(
            IRequest<TResponse> request,
            CancellationToken cancellationToken = default)
        {
            if (typeof(TResponse) == typeof(Update[]))
            {
                Update[] batch;
                lock (_batchLock)
                {
                    if (_yielded)
                    {
                        batch = _emptyBatch;
                    }
                    else
                    {
                        _yielded = true;
                        batch = _firstBatch;
                    }
                }

                return Task.FromResult((TResponse) (object) batch);
            }

            return Task.FromResult(default(TResponse)!);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Returns <c>true</c> as a deterministic no-op default.
        /// </remarks>
        public Task<bool> TestApi(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        /// <remarks>
        /// No-op: the scenario flows under test do not download files.
        /// </remarks>
        public Task DownloadFile(
            string filePath,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <remarks>
        /// No-op: the scenario flows under test do not download files.
        /// </remarks>
        public Task DownloadFile(
            TGFile file,
            Stream destination,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
