using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
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
                errorHandler: (ex, token) => updateHandler.HandleErrorAsync(_botClient, ex, token)
            );
        }

        #endregion
    }
}
