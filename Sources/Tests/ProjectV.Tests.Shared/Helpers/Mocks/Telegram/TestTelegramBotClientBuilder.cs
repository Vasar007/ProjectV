using System.Threading;
using Acolyte.Assertions;
using Telegram.Bot;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace ProjectV.Tests.Shared.Helpers.Mocks.Telegram
{
    /// <summary>
    /// Builder for <see cref="ITelegramBotClient" /> test doubles backed by
    /// <see cref="NSubstitute" /> (Decision D-33). Lets a test inject a
    /// deterministic bot-client into the
    /// <see cref="ProjectV.TelegramBotWebService" /> host without contacting
    /// the live Telegram API.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The webhook scenario tests in
    /// <c>ProjectV.TelegramBotWebService.Tests/Scenarios/Webhook/</c> use
    /// <see cref="CreateWithoutSetup" /> — the bot handler may call
    /// <c>SendMessage</c> / <c>SendTextMessageAsync</c> on the client but the
    /// substitute swallows it; the test asserts on the controller's response
    /// status, not on outgoing bot calls.
    /// </para>
    /// <para>
    /// The polling scenario tests in <c>02-12-telegram-polling-tests</c> use
    /// <see cref="WithUpdateSequence" /> — Telegram.Bot 22.x routes the
    /// <c>ReceiveAsync</c> extension method through
    /// <see cref="ITelegramBotClient.SendRequest{TResponse}" /> with a
    /// <c>GetUpdatesRequest</c> / response type
    /// <see cref="Update" /><c>[]</c>. Substituting <c>SendRequest&lt;Update[]&gt;</c>
    /// is the natural test seam: the first poll yields the configured
    /// sequence; subsequent polls yield an empty array (the receiver loops
    /// until the supplied <see cref="System.Threading.CancellationToken" />
    /// signals).
    /// </para>
    /// </remarks>
    public sealed class TestTelegramBotClientBuilder
    {
        private readonly List<Update> _updateSequence = new List<Update>();

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="TestTelegramBotClientBuilder" /> class. No behavior is
        /// configured until <see cref="WithUpdateSequence" /> is called or
        /// <see cref="Build" /> is invoked.
        /// </summary>
        public TestTelegramBotClientBuilder()
        {
        }

        /// <summary>
        /// Convenience factory returning a bare-bones
        /// <see cref="ITelegramBotClient" /> substitute. The substitute
        /// silently absorbs every <c>SendRequest</c> / extension-method call
        /// (<c>SendMessage</c>, <c>SetWebhook</c>, etc.) without contacting
        /// the real Telegram API — sufficient for webhook scenario tests
        /// where the test asserts on the controller response, not on the
        /// outgoing bot calls.
        /// </summary>
        public static ITelegramBotClient CreateWithoutSetup()
        {
            return new TestTelegramBotClientBuilder().Build();
        }

        /// <summary>
        /// Configures the substitute to yield the supplied
        /// <see cref="Update" /> sequence on the first
        /// <c>SendRequest&lt;Update[]&gt;</c> call (i.e. the first poll).
        /// Subsequent polls receive an empty array — the long-polling loop
        /// will keep going until the caller's
        /// <see cref="System.Threading.CancellationToken" /> signals.
        /// </summary>
        /// <param name="updates">
        /// Updates to yield on the first poll. Must not be <c>null</c>;
        /// null elements are rejected.
        /// </param>
        /// <returns>This builder, for fluent chaining.</returns>
        public TestTelegramBotClientBuilder WithUpdateSequence(IEnumerable<Update> updates)
        {
            updates.ThrowIfNull(nameof(updates));

            foreach (Update update in updates)
            {
                update.ThrowIfNull(nameof(updates));
                _updateSequence.Add(update);
            }

            return this;
        }

        /// <summary>
        /// Builds the <see cref="ITelegramBotClient" /> substitute. If
        /// <see cref="WithUpdateSequence" /> was called, the substitute is
        /// pre-configured so the first <c>SendRequest&lt;Update[]&gt;</c>
        /// call yields the configured sequence and subsequent calls yield an
        /// empty array.
        /// </summary>
        public ITelegramBotClient Build()
        {
            var substitute = Substitute.For<ITelegramBotClient>();

            if (_updateSequence.Count > 0)
            {
                Update[] firstBatch = _updateSequence.ToArray();
                Update[] emptyBatch = Array.Empty<Update>();
                bool yielded = false;

                substitute
                    .SendRequest(Arg.Any<IRequest<Update[]>>(), Arg.Any<CancellationToken>())
                    .Returns(_ =>
                    {
                        if (yielded)
                        {
                            return Task.FromResult(emptyBatch);
                        }

                        yielded = true;
                        return Task.FromResult(firstBatch);
                    });
            }

            return substitute;
        }
    }
}
