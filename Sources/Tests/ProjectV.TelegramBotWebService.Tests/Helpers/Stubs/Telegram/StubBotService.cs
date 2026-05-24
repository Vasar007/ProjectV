using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProjectV.TelegramBotWebService.Tests.Helpers.Stubs.Telegram
{
    /// <summary>
    /// Scenario-test stub for <see cref="IBotService" />. Owns a real (or
    /// fake) <see cref="ITelegramBotClient" /> and returns deterministic
    /// no-op completions for the API surface exercised by scenario tests.
    /// Per the <c>create-tests</c> scenario rules, scenario tests must use
    /// stubs (named <c>Stub{DependencyName}</c>) rather than NSubstitute
    /// mocks for types they own.
    /// </summary>
    /// <remarks>
    /// This stub records the name of every <see cref="IBotService" /> method
    /// that the production code invokes, in invocation order. Scenario tests
    /// use <see cref="CalledMethodNames" /> to assert on the production
    /// handler chain's interaction shape without relying on NSubstitute's
    /// call-tracking API. The underlying <see cref="ITelegramBotClient" /> is
    /// provided by the test composition root and is not disposed here. Because
    /// the polling path can drive <c>SendMessageAsync</c> from concurrent
    /// handler invocations, the call list is protected by a lock so that
    /// invocation order is deterministic and no records are lost.
    /// </remarks>
    public sealed class StubBotService : IBotService
    {
        private readonly object _callsLock = new();

        private readonly List<string> _calledMethodNames = new();

        /// <summary>
        /// Gets a snapshot of the <see cref="IBotService" /> method names
        /// that the production code has invoked on this stub, in invocation
        /// order. Scenario tests read this property to assert on the
        /// interaction shape of the production handler chain (e.g., that
        /// <c>SendMessageAsync</c> was called the expected number of times).
        /// The returned list is a point-in-time copy; it does not update after
        /// capture.
        /// </summary>
        public IReadOnlyList<string> CalledMethodNames
        {
            get
            {
                lock (_callsLock)
                {
                    return _calledMethodNames.AsReadOnly();
                }
            }
        }

        /// <inheritdoc />
        public ITelegramBotClient BotClient { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StubBotService" />
        /// class with the supplied bot client.
        /// </summary>
        /// <param name="botClient">
        /// The <see cref="ITelegramBotClient" /> that this stub exposes via
        /// <see cref="BotClient" />. Must not be <c>null</c>.
        /// </param>
        public StubBotService(ITelegramBotClient botClient)
        {
            BotClient = botClient.ThrowIfNull(nameof(botClient));
        }

        /// <inheritdoc />
        /// <remarks>
        /// Scenario tests drive update delivery through the bot-client
        /// stub's <c>GetUpdates</c> implementation, not through this method.
        /// Returns an empty list as a deterministic no-op default.
        /// </remarks>
        public Task<IReadOnlyList<Update>> GetUpdatesAsync(
            int? offset = default,
            int? limit = default,
            int? timeout = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            CancellationToken cancellationToken = default)
        {
            lock (_callsLock)
            {
                _calledMethodNames.Add(nameof(GetUpdatesAsync));
            }

            return Task.FromResult<IReadOnlyList<Update>>(System.Array.Empty<Update>());
        }

        /// <inheritdoc />
        public Task SetWebhookAsync(
            string url,
            InputFileStream? certificate = default,
            string? ipAddress = default,
            int? maxConnections = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            bool dropPendingUpdates = default,
            CancellationToken cancellationToken = default)
        {
            lock (_callsLock)
            {
                _calledMethodNames.Add(nameof(SetWebhookAsync));
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DeleteWebhookAsync(
            bool dropPendingUpdates = default,
            CancellationToken cancellationToken = default)
        {
            lock (_callsLock)
            {
                _calledMethodNames.Add(nameof(DeleteWebhookAsync));
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<WebhookInfo> GetWebhookInfoAsync(
            CancellationToken cancellationToken = default)
        {
            lock (_callsLock)
            {
                _calledMethodNames.Add(nameof(GetWebhookInfoAsync));
            }

            return Task.FromResult(new WebhookInfo());
        }

        /// <inheritdoc />
        public Task<Message> SendMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = default,
            ReplyParameters? replyParameters = default,
            ReplyMarkup? replyMarkup = default,
            LinkPreviewOptions? linkPreviewOptions = default,
            int? messageThreadId = default,
            IEnumerable<MessageEntity>? entities = default,
            bool disableNotification = default,
            bool protectContent = default,
            string? messageEffectId = default,
            string? businessConnectionId = default,
            bool allowPaidBroadcast = default,
            CancellationToken cancellationToken = default)
        {
            lock (_callsLock)
            {
                _calledMethodNames.Add(nameof(SendMessageAsync));
            }

            return Task.FromResult(new Message());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Stub is stateless from a resource-ownership perspective —
            // the underlying ITelegramBotClient is owned by the test
            // composition root, not this stub.
        }
    }
}
