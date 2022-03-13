using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
