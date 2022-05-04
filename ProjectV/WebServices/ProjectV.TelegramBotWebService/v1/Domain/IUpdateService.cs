using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IUpdateService : IDisposable
    {
        Task ProcessUpdateMessage(Update update);
    }
}
