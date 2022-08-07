using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Receivers
{
    public abstract class AsyncUpdateReceiverBase : IUpdateReceiver
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

        #region IUpdateReceiver Implementation

        public async Task ReceiveAsync(IUpdateHandler updateHandler,
            CancellationToken cancellationToken = default)
        {
            var asyncUpdateEnumerator = CreateAsyncEnumerator(updateHandler);

            await foreach (var update in asyncUpdateEnumerator.WithCancellation(cancellationToken))
            {
                await updateHandler.HandleUpdateAsync(_botClient, update, cancellationToken);
            }
        }

        #endregion

        protected abstract IAsyncEnumerable<Update> CreateAsyncEnumerator(
            IUpdateHandler updateHandler);
    }
}
