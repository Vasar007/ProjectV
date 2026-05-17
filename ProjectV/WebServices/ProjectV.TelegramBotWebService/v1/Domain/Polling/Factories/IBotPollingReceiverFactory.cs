using ProjectV.TelegramBotWebService.Options;
using Telegram.Bot.Polling;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Factories
{
    /// <summary>
    /// Factory to create <see cref="ReceiverOptions" /> for polling.
    /// </summary>
    /// <remarks>
    /// NOTE: This interface is kept for potential future use. As of Telegram.Bot 22.10,
    /// the IUpdateReceiver abstraction has been removed from the library. Polling is now
    /// performed via <see cref="TelegramBotClientExtensions.ReceiveAsync" /> directly.
    /// </remarks>
    public interface IBotPollingReceiverFactory
    {
        ReceiverOptions Create(BotPollingOptions options);
    }
}
