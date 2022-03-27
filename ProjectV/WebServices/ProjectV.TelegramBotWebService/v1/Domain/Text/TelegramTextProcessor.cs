using System.Collections.Generic;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.TelegramBotWebService.Config;

namespace ProjectV.TelegramBotWebService.v1.Domain.Text
{
    public sealed class TelegramTextProcessor : ITelegramTextProcessor
    {
        private readonly TelegramBotWebServiceSettings _settings;

        private string EndlineSeparator => _settings.EndlineSeparator;


        public TelegramTextProcessor(
                 IOptions<TelegramBotWebServiceSettings> settings)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));
        }

        #region ITelegramTextProcessor Implementation

        public string JoinWithNewLineSeparator(IEnumerable<string> messages)
        {
            return string.Join(EndlineSeparator, messages);
        }

        public IReadOnlyList<string> ParseAsSeparateLines(string message)
        {
            var data = message.Split(EndlineSeparator);
            return data;
        }

        public string ParseCommand(string message)
        {
            IReadOnlyList<string> firstLine = message.Split(' ');
            string command = firstLine[0];
            return command;
        }

        #endregion
    }
}
