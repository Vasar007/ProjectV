using System.ComponentModel.DataAnnotations;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class TelegramBotWebServiceOptions : IOptions
    {
        [Required]
        public BotOptions Bot { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ServiceApiUrl { get; set; } = default!;


        public TelegramBotWebServiceOptions()
        {
        }
    }
}
