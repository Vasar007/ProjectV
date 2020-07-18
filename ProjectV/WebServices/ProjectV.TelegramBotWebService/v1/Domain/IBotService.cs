using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
