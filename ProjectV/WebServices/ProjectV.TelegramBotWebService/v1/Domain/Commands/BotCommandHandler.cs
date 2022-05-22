using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.TelegramBotWebService.Options;

namespace ProjectV.TelegramBotWebService.v1.Domain.Commands
{
    public sealed class BotCommandHandler : IBotCommandHandler
    {
        private readonly TelegramBotWebServiceOptions _options;


        public BotCommandHandler(
            IOptions<TelegramBotWebServiceOptions> options)
        {
            _options = options.GetCheckedValue();
        }
    }
}
