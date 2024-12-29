using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;
using ProjectV.TelegramBotWebService.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public sealed class BotService : IBotService
    {
        private readonly ProjectVServiceOptions _serviceOptions;
        private readonly TelegramBotWebServiceOptions _botServiceOptions;

        private readonly HttpClient _httpClient;

        private readonly bool _continueOnCapturedContext;

        private HttpClientOptions HcOptions => _serviceOptions.HttpClient;
        private string BotToken => _botServiceOptions.Bot.Token;

        /// <inheritdoc />
        public ITelegramBotClient BotClient { get; }


        public BotService(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serviceOptions,
            IOptions<TelegramBotWebServiceOptions> botServiceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _serviceOptions = serviceOptions.GetCheckedValue();
            _botServiceOptions = botServiceOptions.GetCheckedValue();

            try
            {
                _httpClient = httpClientFactory.CreateClientWithOptions(HcOptions);
                _continueOnCapturedContext = false;

                BotClient = new TelegramBotClient(BotToken, _httpClient);
            }
            catch (Exception)
            {
                _httpClient.DisposeClient(HcOptions);
                throw;
            }
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _httpClient.DisposeClient(HcOptions);

            _disposed = true;
        }

        #endregion

        #region IBotService Implementation

        /// <inheritdoc />
        public async Task<IReadOnlyList<Update>> GetUpdatesAsync(
            int? offset = null,
            int? limit = null,
            int? timeout = null,
            IEnumerable<UpdateType>? allowedUpdates = null,
            CancellationToken cancellationToken = default)
        {
            return await InternalCall(
                () => BotClient.GetUpdates(
                    offset: offset,
                    limit: limit,
                    timeout: timeout,
                    allowedUpdates: allowedUpdates,
                    cancellationToken: cancellationToken
                )
            ).ConfigureAwait(_continueOnCapturedContext);
        }

        /// <inheritdoc />
        public async Task SetWebhookAsync(
            string url,
            InputFileStream? certificate = null,
            string? ipAddress = null,
            int? maxConnections = null,
            IEnumerable<UpdateType>? allowedUpdates = null,
            bool dropPendingUpdates = false,
            CancellationToken cancellationToken = default)
        {
            await InternalCall(
               () => BotClient.SetWebhook(
                    url: url,
                    certificate: certificate,
                    ipAddress: ipAddress,
                    maxConnections: maxConnections,
                    allowedUpdates: allowedUpdates,
                    dropPendingUpdates: dropPendingUpdates,
                    cancellationToken: cancellationToken
                )
            ).ConfigureAwait(_continueOnCapturedContext);
        }

        /// <inheritdoc />
        public async Task DeleteWebhookAsync(
            bool dropPendingUpdates = false,
            CancellationToken cancellationToken = default)
        {
            await InternalCall(
                () => BotClient.DeleteWebhook(
                    dropPendingUpdates: dropPendingUpdates,
                    cancellationToken: cancellationToken
                )
            ).ConfigureAwait(_continueOnCapturedContext);
        }

        /// <inheritdoc />
        public async Task<WebhookInfo> GetWebhookInfoAsync(
            CancellationToken cancellationToken = default)
        {
            return await InternalCall(
                 () => BotClient.GetWebhookInfo(
                     cancellationToken: cancellationToken
                 )
             ).ConfigureAwait(_continueOnCapturedContext);
        }

        /// <inheritdoc />
        public async Task<Message> SendMessageAsync(
            ChatId chatId,
            string text,
            ParseMode parseMode = ParseMode.None,
            ReplyParameters? replyParameters = null,
            IReplyMarkup? replyMarkup = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            int? messageThreadId = null,
            IEnumerable<MessageEntity>? entities = null,
            bool disableNotification = false,
            bool protectContent = false,
            string? messageEffectId = null,
            string? businessConnectionId = null,
            bool allowPaidBroadcast = false,
            CancellationToken cancellationToken = default)
        {
            return await InternalCall(
                () => BotClient.SendMessage(
                    chatId: chatId,
                    text: text,
                    parseMode: parseMode,
                    replyParameters: replyParameters,
                    replyMarkup: replyMarkup,
                    linkPreviewOptions: linkPreviewOptions,
                    messageThreadId: messageThreadId,
                    entities: entities,
                    disableNotification: disableNotification,
                    protectContent: protectContent,
                    messageEffectId: messageEffectId,
                    businessConnectionId: businessConnectionId,
                    allowPaidBroadcast: allowPaidBroadcast,
                    cancellationToken: cancellationToken
                )
            ).ConfigureAwait(_continueOnCapturedContext);
        }

        #endregion

        private Task InternalCall(Func<Task> action)
        {
            return InternalCall(async () =>
            {
                await action()
                    .ConfigureAwait(_continueOnCapturedContext);
                return default(bool);
            });
        }

        private async Task<TResult> InternalCall<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                return await action()
                    .ConfigureAwait(_continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                throw ReconstructExceptionIfNeeded(ex);
            }
        }

        private static Exception ReconstructExceptionIfNeeded(Exception ex)
        {
            return ex switch
            {
                ApiRequestException apiEx => new Exception(ConstructMessageFrom(apiEx), apiEx),

                _ => ex
            };
        }

        private static string ConstructMessageFrom(ApiRequestException ex)
        {
            var requestParameters = ex.Parameters;

            string parameters = requestParameters is null
                ? "No parameters were specified"
                : $"[MigrateToChatId: {requestParameters.MigrateToChatId.ToStringNullSafe()}, " +
                  $"RetryAfter: {requestParameters.RetryAfter.ToStringNullSafe()}]";

            return $"Telegram API exception: {ex.Message} ({ex.ErrorCode}). Parameters: {parameters}.";
        }
    }
}
