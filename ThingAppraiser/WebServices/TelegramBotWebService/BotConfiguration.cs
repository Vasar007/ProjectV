namespace ThingAppraiser.TelegramBotWebService
{
    public class BotConfiguration
    {
        public string BotToken { get; } = EnvironmentVariablesParser.GetValue("BotToken");

        public bool UseProxy { get; set; }

        public string Socks5Host { get; set; }

        public int Socks5Port { get; set; }


        public BotConfiguration()
        {
        }
    }
}
