using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Receivers
{
    public sealed class AsyncBlockingUpdateReceiver : AsyncUpdateReceiverBase
    {

        public AsyncBlockingUpdateReceiver(
            ITelegramBotClient botClient,
            ReceiverOptions? receiverOptions = default)
            : base(botClient, receiverOptions)
        {
        }

        #region UpdateReceiverBase Overridden Methods

        protected override IAsyncEnumerable<Update> CreateAsyncEnumerator(
            IUpdateHandler updateHandler)
        {
            return new BlockingUpdateReceiver(
                botClient: _botClient,
                receiverOptions: _receiverOptions,
                pollingErrorHandler: (ex, token) => updateHandler.HandlePollingErrorAsync(_botClient, ex, token)
            );
        }

        #endregion
    }
}
