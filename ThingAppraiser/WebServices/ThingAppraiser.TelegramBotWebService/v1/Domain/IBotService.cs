using Telegram.Bot;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public interface IBotService
    {
        TelegramBotClient Client { get; }
    }
}
