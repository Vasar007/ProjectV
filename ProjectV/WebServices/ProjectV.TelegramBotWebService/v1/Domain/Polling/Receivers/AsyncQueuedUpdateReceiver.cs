using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Receivers
{
    /// <summary>
    /// Queued update receiver implementation.
    /// </summary>
    /// <remarks>
    /// NOTE: As of Telegram.Bot 22.10, <c>QueuedUpdateReceiver</c> has been removed from the
    /// library. This class is kept for source compatibility but is no longer wired into the polling
    /// pipeline. Polling is performed via
    /// <see cref="TelegramBotClientExtensions.ReceiveAsync" /> directly.
    /// </remarks>
    public sealed class AsyncQueuedUpdateReceiver : AsyncUpdateReceiverBase
    {
        public AsyncQueuedUpdateReceiver(
            ITelegramBotClient botClient,
            ReceiverOptions? receiverOptions = default)
            : base(botClient, receiverOptions)
        {
        }

        #region UpdateReceiverBase Overridden Methods

        protected override async IAsyncEnumerable<Update> CreateAsyncEnumerator(
            IUpdateHandler updateHandler)
        {
            // NOTE: QueuedUpdateReceiver was removed in Telegram.Bot 22.10.
            // This method is no longer called from the polling pipeline.
            await System.Threading.Tasks.Task.CompletedTask;
            yield break;
        }

        #endregion
    }
}
