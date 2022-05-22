using System.Collections.Generic;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.TelegramBotWebService.Options;

namespace ProjectV.TelegramBotWebService.v1.Domain.Text
{
    public sealed class TelegramTextProcessor : ITelegramTextProcessor
    {
        private readonly TelegramBotWebServiceSettings _settings;

        private string NewLineSeparator => _settings.NewLineSeparator;


        public TelegramTextProcessor(
                 IOptions<TelegramBotWebServiceSettings> settings)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));
        }

        #region ITelegramTextProcessor Implementation

        public string JoinWithNewLineLSeparator(IEnumerable<string> messages)
        {
            return string.Join(NewLineSeparator, messages);
        }

        public string TrimNewLineSeparator(string message)
        {
            message.ThrowIfNull(nameof(message));

            return message.Replace(NewLineSeparator, string.Empty);
        }

        public IReadOnlyList<string> ParseAsSeparateLines(string message)
        {
            message.ThrowIfNull(nameof(message));

            var data = message.Split(NewLineSeparator);
            return data;
        }

        public string ParseCommand(string message)
        {
            message.ThrowIfNull(nameof(message));

            IReadOnlyList<string> firstLine = message.Split(' ');
            string command = firstLine[0];
            return command;
        }

        #endregion
    }
}
