namespace ThingAppraiser.TelegramBotWebService
{
    public class BotConfiguration
    {
        private static readonly string _defaultBotToken = "BOT_TOKEN";

        public string BotToken { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("BotToken", _defaultBotToken);

        public bool UseProxy { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }


        public BotConfiguration()
        {
        }
    }
}
