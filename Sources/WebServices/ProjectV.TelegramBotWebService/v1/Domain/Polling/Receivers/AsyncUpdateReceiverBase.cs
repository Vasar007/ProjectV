using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Receivers
{
    /// <summary>
    /// Base class for async update receivers.
    /// </summary>
    /// <remarks>
    /// NOTE: As of Telegram.Bot 22.10, <c>IUpdateReceiver</c> has been removed from the library.
    /// This base class is kept for potential future extension but is no longer wired into the
    /// polling pipeline directly. Polling is performed via
    /// <see cref="TelegramBotClientExtensions.ReceiveAsync" /> on <see cref="ITelegramBotClient" />.
    /// </remarks>
    public abstract class AsyncUpdateReceiverBase
    {
        protected readonly ITelegramBotClient _botClient;

        protected readonly ReceiverOptions? _receiverOptions;


        public AsyncUpdateReceiverBase(
            ITelegramBotClient botClient,
            ReceiverOptions? receiverOptions = default)
        {
            _botClient = botClient.ThrowIfNull(nameof(botClient));
            _receiverOptions = receiverOptions;
        }

        public async Task ReceiveAsync(IUpdateHandler updateHandler,
            CancellationToken cancellationToken = default)
        {
            var asyncUpdateEnumerator = CreateAsyncEnumerator(updateHandler);

            await foreach (var update in asyncUpdateEnumerator.WithCancellation(cancellationToken))
            {
                await updateHandler.HandleUpdateAsync(_botClient, update, cancellationToken);
            }
        }

        protected abstract IAsyncEnumerable<Update> CreateAsyncEnumerator(
            IUpdateHandler updateHandler);
    }
}
