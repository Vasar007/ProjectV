using System.Collections.Generic;

namespace ProjectV.TelegramBotWebService.v1.Domain.Text
{
    public interface ITelegramTextProcessor
    {
        string JoinWithNewLineLSeparator(IEnumerable<string> messages);
        string TrimNewLineSeparator(string message);

        IReadOnlyList<string> ParseAsSeparateLines(string message);
        string ParseCommand(string message);
    }
}
