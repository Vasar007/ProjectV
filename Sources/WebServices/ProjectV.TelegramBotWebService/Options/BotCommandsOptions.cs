using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotCommandsOptions : IOptions
    {
        public string Prefix { get; init; } = "/";

        public string Start { get; init; } = "start";
        public string StartCommand => MakeCommand(Start);

        public string Services { get; init; } = "services";
        public string ServicesCommand => MakeCommand(Services);

        public string Request { get; init; } = "request";
        public string RequestCommand => MakeCommand(Request);

        public string Cancel { get; init; } = "cancel";
        public string CancelCommand => MakeCommand(Cancel);

        public string Help { get; init; } = "help";
        public string HelpCommand => MakeCommand(Help);


        public BotCommandsOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            Prefix.ThrowIfNullOrWhiteSpace(nameof(Prefix));
            Start.ThrowIfNullOrWhiteSpace(nameof(Start));
            Services.ThrowIfNullOrWhiteSpace(nameof(Services));
            Request.ThrowIfNullOrWhiteSpace(nameof(Request));
            Cancel.ThrowIfNullOrWhiteSpace(nameof(Cancel));
            Help.ThrowIfNullOrWhiteSpace(nameof(Help));
        }

        #endregion

        private string MakeCommand(string command)
        {
            return $"{Prefix}{command}";
        }
    }
}
