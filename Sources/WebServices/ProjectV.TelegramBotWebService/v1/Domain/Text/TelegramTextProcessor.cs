using System.Collections.Generic;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.TelegramBotWebService.Options;

namespace ProjectV.TelegramBotWebService.v1.Domain.Text
{
    public sealed class TelegramTextProcessor : ITelegramTextProcessor
    {
        private readonly TelegramBotWebServiceOptions _options;

        private string NewLineSeparator => _options.Bot.NewLineSeparator;


        public TelegramTextProcessor(
            IOptions<TelegramBotWebServiceOptions> options)
        {
            _options = options.GetCheckedValue();
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
