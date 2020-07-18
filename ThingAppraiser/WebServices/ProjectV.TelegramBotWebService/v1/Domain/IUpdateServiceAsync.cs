using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public interface IUpdateServiceAsync
    {
        Task ProcessUpdateMessage(Update update);
    }
}
