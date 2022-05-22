using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    // TODO: make this DTO immutable.
    public sealed class BotOptions : IOptions
    {
        public string BotToken { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("BotToken", "BOT_TOKEN");
        public bool UseProxy { get; set; }

        public string Socks5Host { get; set; } = default!;

        [Range(0, 65535, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Socks5Port { get; set; }

        public BotOptions()
        {
        }
    }
}
