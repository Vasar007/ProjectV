using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public interface IBotService : IDisposable
    {
        /// <inheritdoc cref="TelegramBotClientExtensions.GetUpdatesAsync" />
        Task<IReadOnlyList<Update>> GetUpdatesAsync(
            int? offset = default,
            int? limit = default,
            int? timeout = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.SetWebhookAsync" />
        Task SetWebhookAsync(
            string url,
            InputFileStream? certificate = default,
            string? ipAddress = default,
            int? maxConnections = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            bool? dropPendingUpdates = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.DeleteWebhookAsync" />
        Task DeleteWebhookAsync(
            bool? dropPendingUpdates = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.GetWebhookInfoAsync" />
        Task<WebhookInfo> GetWebhookInfoAsync(
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.SendTextMessageAsync" />
        Task<Message> SendTextMessageAsync(
            ChatId chatId,
            string text,
            ParseMode? parseMode = default,
            IEnumerable<MessageEntity>? entities = default,
            bool? disableWebPagePreview = default,
            bool? disableNotification = default,
            int? replyToMessageId = default,
            bool? allowSendingWithoutReply = default,
            IReplyMarkup? replyMarkup = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientPollingExtensions.ReceiveAsync(ITelegramBotClient, IUpdateHandler, ReceiverOptions?, CancellationToken)" />
        public Task ReceiveAsync(
            IUpdateHandler updateHandler,
            ReceiverOptions? receiverOptions = default,
            CancellationToken cancellationToken = default
        );
    }
}
