using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public interface IBotService : IDisposable
    {
        /// <inheritdoc cref="ITelegramBotClient" />
        ITelegramBotClient BotClient { get; }


        /// <inheritdoc cref="TelegramBotClientExtensions.GetUpdates" />
        Task<IReadOnlyList<Update>> GetUpdatesAsync(
            int? offset = default,
            int? limit = default,
            int? timeout = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.SetWebhook" />
        Task SetWebhookAsync(
            string url,
            InputFileStream? certificate = default,
            string? ipAddress = default,
            int? maxConnections = default,
            IEnumerable<UpdateType>? allowedUpdates = default,
            bool dropPendingUpdates = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.DeleteWebhook" />
        Task DeleteWebhookAsync(
            bool dropPendingUpdates = default,
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.GetWebhookInfo" />
        Task<WebhookInfo> GetWebhookInfoAsync(
            CancellationToken cancellationToken = default
        );

        /// <inheritdoc cref="TelegramBotClientExtensions.SendMessage" />
        Task<Message> SendMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = default,
            ReplyParameters? replyParameters = default,
            IReplyMarkup? replyMarkup = default,
            LinkPreviewOptions? linkPreviewOptions = default,
            int? messageThreadId = default,
            IEnumerable<MessageEntity>? entities = default,
            bool disableNotification = default,
            bool protectContent = default,
            string? messageEffectId = default,
            string? businessConnectionId = default,
            bool allowPaidBroadcast = default,
            CancellationToken cancellationToken = default
        );
    }
}
