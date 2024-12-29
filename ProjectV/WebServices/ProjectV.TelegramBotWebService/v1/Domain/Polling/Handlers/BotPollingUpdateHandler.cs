using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Handlers
{
    public sealed class BotPollingUpdateHandler : IBotPollingUpdateHandler
    {
        // Contract: "_updateService" contains the same "botClient" as we received through
        // parameters in all its methods.

        private readonly IUpdateService _updateService;


        public BotPollingUpdateHandler(
            IUpdateService updateService)
        {
            _updateService = updateService.ThrowIfNull(nameof(updateService));
        }

        #region IBotPollingUpdateHandler Implementation

        /// <inheritdoc />
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            await _updateService.HandleUpdateAsync(update, cancellationToken);
        }

        /// <inheritdoc />
        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            await _updateService.HandlePollingErrorAsync(exception, cancellationToken);
        }

        /// <inheritdoc />
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            HandleErrorSource source, CancellationToken cancellationToken)
        {
            await _updateService.HandleErrorAsync(exception, source, cancellationToken);
        }

        #endregion
    }
}
