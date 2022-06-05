using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Handlers
{
    public sealed class BotPollingUpdateHandler : IBotPollingUpdateHandler
    {
        // Contract: "_updateService" contains the same "botClient" as we received through
        // parameters in "HandleUpdateAsync" and "HandleErrorAsync" methods.

        private readonly IUpdateService _updateService;


        public BotPollingUpdateHandler(
            IUpdateService updateService)
        {
            _updateService = updateService.ThrowIfNull(nameof(updateService));
        }

        #region IBotPollingUpdateHandler Implementation

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            await _updateService.ProcessUpdateRequestAsync(update, cancellationToken);
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
            CancellationToken cancellationToken)
        {
            await _updateService.HandleErrorAsync(exception, cancellationToken);
        }

        #endregion
    }
}
