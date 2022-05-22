using System;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public interface IBotService : IDisposable
    {
        ITelegramBotClient Client { get; }
    }
}
