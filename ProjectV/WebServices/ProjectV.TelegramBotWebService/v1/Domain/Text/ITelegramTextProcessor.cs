using System.Collections.Generic;

namespace ProjectV.TelegramBotWebService.v1.Domain.Text
{
    public interface ITelegramTextProcessor
    {
        string JoinWithNewLineSeparator(IEnumerable<string> messages);
        IReadOnlyList<string> ParseAsSeparateLines(string message);
        string ParseCommand(string message);
    }
}
