using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotCommandsOptions : IOptions
    {
        public string Prefix { get; set; } = "/";

        public string Start { get; set; } = "start";
        public string StartCommand => MakeCommand(Start);

        public string Services { get; set; } = "services";
        public string ServicesCommand => MakeCommand(Services);

        public string Request { get; set; } = "request";
        public string RequestCommand => MakeCommand(Request);

        public string Cancel { get; set; } = "cancel";
        public string CancelCommand => MakeCommand(Cancel);

        public string Help { get; set; } = "help";
        public string HelpCommand => MakeCommand(Help);


        public BotCommandsOptions()
        {
        }

        private string MakeCommand(string command)
        {
            return $"{Prefix}{command}";
        }
    }
}
