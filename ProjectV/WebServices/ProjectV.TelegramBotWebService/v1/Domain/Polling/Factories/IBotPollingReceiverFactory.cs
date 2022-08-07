using ProjectV.TelegramBotWebService.Options;
using Telegram.Bot.Polling;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Factories
{
    public interface IBotPollingReceiverFactory
    {
        IUpdateReceiver Create(BotPollingOptions options);
    }
}
